# Tennis Club Manager – Implementacijski plan

## Sadržaj

1. [Pregled projekta](#1-pregled-projekta)
2. [Tehnološki stog](#2-tehnološki-stog)
3. [Pregled arhitekture](#3-pregled-arhitekture)
4. [Shema baze podataka](#4-shema-baze-podataka)
5. [API endpointi](#5-api-endpointi)
6. [Faze razvoja](#6-faze-razvoja)
7. [Struktura projekta](#7-struktura-projekta)
8. [Buduće nadogradnje](#8-buduće-nadogradnje)

---

## 1. Pregled projekta

**Tennis Club Manager** je višestanična (multi-tenant) aplikacija za upravljanje tenis klubovima. Jedan sustav podržava više nezavisnih klubova, a svaki klub ima vlastite terene, članove, rezervacije, mečeve i lige.

### Ključne karakteristike

- Svaki klub je odvojena organizacijska jedinica unutar sustava
- Igrač može biti član više klubova istovremeno
- Uloge korisnika razlikuju se po kontekstu kluba (korisnik može biti admin u jednom klubu, a igrač u drugom)
- Pristup sustavu prilagođen je svakoj ulozi: admin, trener, igrač, gost
- Aplikacija je dostupna putem weba i mobilnih uređaja

---

## 2. Tehnološki stog

| Sloj | Tehnologija |
|------|------------|
| Backend API | ASP.NET Core Web API (.NET 8+) |
| Baza podataka | Supabase (PostgreSQL) |
| ORM | Entity Framework Core + Npgsql provider |
| Web frontend | Blazor WebAssembly ili React (TBD) |
| Mobilne aplikacije | .NET MAUI |
| Autentikacija | Supabase Auth + JWT |
| Pohrana datoteka | Supabase Storage |
| Notifikacije | Email + Push notifikacije |

---

## 3. Pregled arhitekture

Aplikacija slijedi **API-first** pristup: svi klijenti (web, mobilni) komuniciraju isključivo s ASP.NET Core Web API-jem.

```
┌──────────────────────────────────────────────────────────┐
│                        Klijenti                          │
│                                                          │
│   ┌─────────────────┐        ┌──────────────────────┐   │
│   │   Web frontend   │        │  Mobilna aplikacija  │   │
│   │ (Blazor / React) │        │     (.NET MAUI)      │   │
│   └────────┬────────┘        └──────────┬───────────┘   │
└────────────┼─────────────────────────────┼───────────────┘
             │  HTTPS / REST + JWT          │
             ▼                             ▼
┌──────────────────────────────────────────────────────────┐
│              ASP.NET Core Web API (.NET 8+)              │
│                                                          │
│   ┌───────────┐  ┌───────────┐  ┌───────────────────┐  │
│   │   Auth    │  │  Domain   │  │  Notification     │  │
│   │ Middleware│  │  Logic    │  │  Service          │  │
│   └───────────┘  └─────┬─────┘  └───────────────────┘  │
└───────────────────────┼──────────────────────────────────┘
                        │
             ┌──────────▼──────────┐
             │      Supabase       │
             │                     │
             │  ┌───────────────┐  │
             │  │  PostgreSQL   │  │
             │  │  (baza pod.)  │  │
             │  └───────────────┘  │
             │  ┌───────────────┐  │
             │  │     Auth      │  │
             │  └───────────────┘  │
             │  ┌───────────────┐  │
             │  │    Storage    │  │
             │  │ (slike, dok.) │  │
             │  └───────────────┘  │
             └─────────────────────┘
```

### Višestaničnost (Multi-tenancy)

Višestaničnost se ostvaruje na razini podataka: svaki entitet koji pripada klubu ima `club_id` strani ključ. Korisnici mogu imati različite uloge u različitim klubovima putem tablice `ClubMembers`.

---

## 4. Shema baze podataka

### 4.1 Dijagram relacija (ERD – tekstualni prikaz)

```
Users ──────────────────────── ClubMembers ─────────────── Clubs
  │                                  │                        │
  │                                  │ (role: admin,          │
  │                                  │  coach, player)        │
  │                           Reservations                 Courts
  │                                  │                        │
  │                                  └──────────── CourtSettings
  │
  ├──────────── Matches ──────────── MatchResults
  │                  │                    │
  │                  └──── Sets           │
  │
  └──────────── LeagueParticipants ─ Leagues ──── LeagueMatches
                                         │
                                     Tournaments

Notifications (vezano uz Reservations i Matches)
```

### 4.2 Definicije tablica

#### `users`
> Upravljano putem Supabase Auth. Prošireno s dodatnim podacima.

```sql
CREATE TABLE users (
    id          UUID PRIMARY KEY,           -- Supabase Auth UID
    email       TEXT NOT NULL UNIQUE,
    first_name  TEXT NOT NULL,
    last_name   TEXT NOT NULL,
    phone       TEXT,
    avatar_url  TEXT,                       -- Supabase Storage URL
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);
```

---

#### `clubs`

```sql
CREATE TABLE clubs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name            TEXT NOT NULL,
    description     TEXT,
    address         TEXT,
    city            TEXT,
    country         TEXT DEFAULT 'HR',
    logo_url        TEXT,                   -- Supabase Storage URL
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

#### `club_members`
> Definira ulogu korisnika unutar pojedinog kluba. Jedan korisnik može imati različite uloge u različitim klubovima.

```sql
CREATE TYPE club_role AS ENUM ('admin', 'coach', 'player');
CREATE TYPE member_status AS ENUM ('pending', 'approved', 'suspended', 'rejected');

CREATE TABLE club_members (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id     UUID NOT NULL REFERENCES clubs(id) ON DELETE CASCADE,
    user_id     UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role        club_role NOT NULL DEFAULT 'player',
    status      member_status NOT NULL DEFAULT 'pending',
    joined_at   TIMESTAMPTZ,
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE (club_id, user_id)
);
```

---

#### `courts`

```sql
CREATE TYPE court_surface AS ENUM ('clay', 'hard', 'grass', 'carpet');
CREATE TYPE court_environment AS ENUM ('indoor', 'outdoor');

CREATE TABLE courts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id         UUID NOT NULL REFERENCES clubs(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,           -- npr. "Teren 1", "Teren A"
    surface         court_surface NOT NULL,
    environment     court_environment NOT NULL,
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

#### `court_settings`
> Postavke rezervacija koje admin definira po klubu (ne po terenu).

```sql
CREATE TABLE court_settings (
    id                          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id                     UUID NOT NULL UNIQUE REFERENCES clubs(id) ON DELETE CASCADE,
    min_reservation_minutes     INT NOT NULL DEFAULT 30,  -- minimum 30 min
    max_reservation_minutes     INT NOT NULL DEFAULT 120,
    cancellation_deadline_hours INT NOT NULL DEFAULT 24,  -- koliko sati prije otkazivanja
    created_at                  TIMESTAMPTZ DEFAULT NOW(),
    updated_at                  TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT min_duration_check CHECK (min_reservation_minutes >= 30)
);
```

---

#### `reservations`

```sql
CREATE TYPE reservation_type AS ENUM ('match', 'training', 'other');
CREATE TYPE reservation_status AS ENUM ('pending', 'confirmed', 'cancelled');

CREATE TABLE reservations (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id         UUID NOT NULL REFERENCES clubs(id) ON DELETE CASCADE,
    court_id        UUID NOT NULL REFERENCES courts(id) ON DELETE CASCADE,
    created_by      UUID NOT NULL REFERENCES users(id),
    reservation_type reservation_type NOT NULL DEFAULT 'other',
    status          reservation_status NOT NULL DEFAULT 'confirmed',
    starts_at       TIMESTAMPTZ NOT NULL,
    ends_at         TIMESTAMPTZ NOT NULL,
    notes           TEXT,
    cancelled_at    TIMESTAMPTZ,
    cancelled_by    UUID REFERENCES users(id),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT valid_duration CHECK (ends_at > starts_at)
);

-- Sudionici rezervacije (npr. oba igrača u meču)
CREATE TABLE reservation_participants (
    reservation_id  UUID NOT NULL REFERENCES reservations(id) ON DELETE CASCADE,
    user_id         UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    PRIMARY KEY (reservation_id, user_id)
);
```

---

#### `matches`

```sql
CREATE TYPE match_type AS ENUM ('singles', 'doubles');

CREATE TABLE matches (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id         UUID NOT NULL REFERENCES clubs(id) ON DELETE CASCADE,
    reservation_id  UUID REFERENCES reservations(id),
    league_id       UUID REFERENCES leagues(id),
    match_type      match_type NOT NULL DEFAULT 'singles',
    played_at       TIMESTAMPTZ,
    notes           TEXT,
    created_by      UUID NOT NULL REFERENCES users(id),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);

-- Igrači meča
CREATE TABLE match_players (
    match_id        UUID NOT NULL REFERENCES matches(id) ON DELETE CASCADE,
    user_id         UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    team            SMALLINT NOT NULL CHECK (team IN (1, 2)), -- tim 1 ili tim 2
    PRIMARY KEY (match_id, user_id)
);
```

---

#### `match_results`

```sql
CREATE TABLE match_results (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    match_id        UUID NOT NULL UNIQUE REFERENCES matches(id) ON DELETE CASCADE,
    winner_team     SMALLINT CHECK (winner_team IN (1, 2)),
    result_text     TEXT,                   -- slobodan unos, npr. "6:3, 7:5"
    entered_by      UUID NOT NULL REFERENCES users(id),
    entered_at      TIMESTAMPTZ DEFAULT NOW()
);

-- Detalji po setovima (opcionalno)
CREATE TABLE match_sets (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    match_id        UUID NOT NULL REFERENCES matches(id) ON DELETE CASCADE,
    set_number      SMALLINT NOT NULL,
    team1_games     SMALLINT,
    team2_games     SMALLINT,
    PRIMARY KEY (match_id, set_number)
);
```

---

#### `leagues`

```sql
CREATE TYPE league_status AS ENUM ('draft', 'active', 'completed', 'cancelled');
CREATE TYPE league_format AS ENUM ('round_robin', 'elimination', 'group_playoff', 'custom');

CREATE TABLE leagues (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    club_id         UUID NOT NULL REFERENCES clubs(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    description     TEXT,
    format          league_format NOT NULL DEFAULT 'round_robin',
    match_type      match_type NOT NULL DEFAULT 'singles',
    status          league_status NOT NULL DEFAULT 'draft',
    starts_at       DATE,
    ends_at         DATE,
    created_by      UUID NOT NULL REFERENCES users(id),
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

#### `league_participants`

```sql
CREATE TABLE league_participants (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    league_id       UUID NOT NULL REFERENCES leagues(id) ON DELETE CASCADE,
    user_id         UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    registered_at   TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE (league_id, user_id)
);
```

---

#### `league_matches`
> Liga mečevi – proširenje tablice `matches` vezanjem na ligu.
>
> Svaki meč u ligi je i redovni meč u tablici `matches` (s ispunjenim `league_id`). Ova tablica čuva dodatne podatke specifične za ligu.

```sql
CREATE TABLE league_matches (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    league_id       UUID NOT NULL REFERENCES leagues(id) ON DELETE CASCADE,
    match_id        UUID NOT NULL UNIQUE REFERENCES matches(id) ON DELETE CASCADE,
    round           SMALLINT,               -- kolo/runda
    group_name      TEXT,                   -- naziv grupe (ako postoji)
    is_required     BOOLEAN DEFAULT TRUE,   -- unos rezultata je obavezan
    created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

#### `notifications`

```sql
CREATE TYPE notification_type AS ENUM (
    'reservation_confirmed',
    'reservation_reminder',
    'reservation_cancelled',
    'match_result_entered',
    'league_match_scheduled',
    'membership_approved',
    'membership_rejected'
);
CREATE TYPE notification_channel AS ENUM ('email', 'push', 'in_app');
CREATE TYPE notification_status AS ENUM ('pending', 'sent', 'failed');

CREATE TABLE notifications (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type            notification_type NOT NULL,
    channel         notification_channel NOT NULL,
    status          notification_status NOT NULL DEFAULT 'pending',
    title           TEXT NOT NULL,
    body            TEXT NOT NULL,
    data            JSONB,                  -- dodatni podaci (npr. reservation_id)
    scheduled_at    TIMESTAMPTZ,
    sent_at         TIMESTAMPTZ,
    created_at      TIMESTAMPTZ DEFAULT NOW()
);
```

---

## 5. API endpointi

Svi endpointi su prefiksirani s `/api/v1`.

### 5.1 Autentikacija (`/auth`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `POST` | `/auth/register` | Registracija novog korisnika | Javno |
| `POST` | `/auth/login` | Prijava (Supabase Auth) | Javno |
| `POST` | `/auth/logout` | Odjava | Autenticirani |
| `POST` | `/auth/refresh` | Obnavljanje JWT tokena | Autenticirani |
| `POST` | `/auth/password-reset` | Zahtjev za reset lozinke | Javno |
| `GET`  | `/auth/me` | Dohvat podataka prijavljenog korisnika | Autenticirani |
| `PUT`  | `/auth/me` | Ažuriranje profila | Autenticirani |

---

### 5.2 Klubovi (`/clubs`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs` | Popis svih aktivnih klubova | Javno |
| `GET` | `/clubs/{id}` | Detalji kluba | Javno |
| `POST` | `/clubs` | Kreiranje novog kluba | Autenticirani |
| `PUT` | `/clubs/{id}` | Ažuriranje kluba | Admin kluba |
| `DELETE` | `/clubs/{id}` | Deaktivacija kluba | Admin kluba |
| `GET` | `/clubs/{id}/settings` | Dohvat postavki kluba (rezervacije) | Član kluba |
| `PUT` | `/clubs/{id}/settings` | Ažuriranje postavki kluba | Admin kluba |

---

### 5.3 Članovi (`/clubs/{clubId}/members`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs/{clubId}/members` | Popis članova kluba | Admin, Coach |
| `POST` | `/clubs/{clubId}/members/request` | Zahtjev za pristupanje klubu | Autenticirani |
| `GET` | `/clubs/{clubId}/members/pending` | Popis zahtjeva na čekanju | Admin kluba |
| `PUT` | `/clubs/{clubId}/members/{userId}/approve` | Odobravanje zahtjeva | Admin kluba |
| `PUT` | `/clubs/{clubId}/members/{userId}/reject` | Odbijanje zahtjeva | Admin kluba |
| `PUT` | `/clubs/{clubId}/members/{userId}/role` | Promjena uloge člana | Admin kluba |
| `DELETE` | `/clubs/{clubId}/members/{userId}` | Uklanjanje člana iz kluba | Admin kluba |
| `GET` | `/clubs/{clubId}/members/{userId}` | Detalji člana | Admin, Coach |

---

### 5.4 Tereni (`/clubs/{clubId}/courts`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs/{clubId}/courts` | Popis terena kluba | Član kluba |
| `GET` | `/clubs/{clubId}/courts/{id}` | Detalji terena | Član kluba |
| `POST` | `/clubs/{clubId}/courts` | Dodavanje terena | Admin kluba |
| `PUT` | `/clubs/{clubId}/courts/{id}` | Ažuriranje terena | Admin kluba |
| `DELETE` | `/clubs/{clubId}/courts/{id}` | Deaktivacija terena | Admin kluba |

---

### 5.5 Rezervacije (`/clubs/{clubId}/reservations`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs/{clubId}/reservations` | Popis rezervacija kluba | Član kluba |
| `GET` | `/clubs/{clubId}/reservations/{id}` | Detalji rezervacije | Sudionik, Admin |
| `POST` | `/clubs/{clubId}/reservations` | Kreiranje rezervacije | Član kluba |
| `PUT` | `/clubs/{clubId}/reservations/{id}` | Ažuriranje rezervacije | Kreator, Admin |
| `DELETE` | `/clubs/{clubId}/reservations/{id}` | Otkazivanje rezervacije | Kreator, Admin |
| `GET` | `/clubs/{clubId}/courts/{courtId}/availability` | Slobodni termini po terenu i datumu | Član kluba |

---

### 5.6 Mečevi (`/clubs/{clubId}/matches`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs/{clubId}/matches` | Popis mečeva kluba | Član kluba |
| `GET` | `/clubs/{clubId}/matches/{id}` | Detalji meča | Član kluba |
| `POST` | `/clubs/{clubId}/matches` | Kreiranje meča | Član kluba |
| `PUT` | `/clubs/{clubId}/matches/{id}` | Ažuriranje meča | Kreator, Admin |
| `DELETE` | `/clubs/{clubId}/matches/{id}` | Brisanje meča | Kreator, Admin |
| `POST` | `/clubs/{clubId}/matches/{id}/result` | Unos rezultata | Član kluba |
| `PUT` | `/clubs/{clubId}/matches/{id}/result` | Ažuriranje rezultata | Unositelj, Admin |

---

### 5.7 Lige (`/clubs/{clubId}/leagues`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/clubs/{clubId}/leagues` | Popis liga kluba | Član kluba |
| `GET` | `/clubs/{clubId}/leagues/{id}` | Detalji lige | Član kluba |
| `POST` | `/clubs/{clubId}/leagues` | Kreiranje lige | Admin, Coach |
| `PUT` | `/clubs/{clubId}/leagues/{id}` | Ažuriranje lige | Kreator, Admin |
| `DELETE` | `/clubs/{clubId}/leagues/{id}` | Brisanje/otkazivanje lige | Kreator, Admin |
| `GET` | `/clubs/{clubId}/leagues/{id}/participants` | Popis sudionika | Član kluba |
| `POST` | `/clubs/{clubId}/leagues/{id}/participants` | Prijava u ligu | Član kluba |
| `DELETE` | `/clubs/{clubId}/leagues/{id}/participants/{userId}` | Odjava iz lige | Sudionik, Admin |
| `GET` | `/clubs/{clubId}/leagues/{id}/matches` | Popis mečeva u ligi | Član kluba |
| `GET` | `/clubs/{clubId}/leagues/{id}/standings` | Ljestvica/poredak | Član kluba |

---

### 5.8 Notifikacije (`/notifications`)

| Metoda | Endpoint | Opis | Pristup |
|--------|----------|------|---------|
| `GET` | `/notifications` | Popis notifikacija prijavljenog korisnika | Autenticirani |
| `PUT` | `/notifications/{id}/read` | Označavanje kao pročitano | Autenticirani |
| `PUT` | `/notifications/read-all` | Označavanje svih kao pročitano | Autenticirani |
| `DELETE` | `/notifications/{id}` | Brisanje notifikacije | Autenticirani |
| `GET` | `/notifications/preferences` | Dohvat preferencija notifikacija | Autenticirani |
| `PUT` | `/notifications/preferences` | Ažuriranje preferencija | Autenticirani |

---

## 6. Faze razvoja

### Faza 1 – MVP: Autentikacija + Klubovi + Članovi + Tereni

**Cilj:** Funkcionalna osnova sustava – korisnici se mogu registrirati, kreirati klubove, upravljati terenima i članovima.

**Zadaci:**
- [ ] Postavljanje Supabase projekta (baza, auth, storage)
- [ ] Inicijalizacija ASP.NET Core Web API projekta (Clean Architecture)
- [ ] Integracija Supabase Auth + JWT middleware
- [ ] Migracije baze podataka (EF Core): `users`, `clubs`, `club_members`, `courts`, `court_settings`
- [ ] CRUD endpointi za klubove
- [ ] Upravljanje članovima (zahtjev za pristup, odobravanje, uloge)
- [ ] CRUD endpointi za terene
- [ ] Upravljanje postavkama kluba (trajanje termina, rok otkazivanja)
- [ ] Osnovna autorizacija po ulogama

**Rezultat:** Admin može kreirati klub, dodavati terene i upravljati članovima.

---

### Faza 2 – Sustav rezervacija s kalendarom

**Cilj:** Članovi mogu rezervirati terene uz poštivanje pravila kluba.

**Zadaci:**
- [ ] Migracije baze: `reservations`, `reservation_participants`
- [ ] CRUD endpointi za rezervacije
- [ ] Validacija rezervacija (trajanje po postavkama kluba, preklapanje termina)
- [ ] Provjera roka otkazivanja pri otkazivanju
- [ ] Endpoint za provjeru dostupnosti terena
- [ ] Osnovna logika za slobodne termine

**Rezultat:** Igrač može vidjeti slobodne termine i kreirati rezervaciju.

---

### Faza 3 – Mečevi i rezultati

**Cilj:** Evidentiranje odigranih mečeva s opcionalnim unosom rezultata.

**Zadaci:**
- [ ] Migracije baze: `matches`, `match_players`, `match_results`, `match_sets`
- [ ] CRUD endpointi za mečeve
- [ ] Endpointi za unos i ažuriranje rezultata
- [ ] Vezanje meča uz rezervaciju (opcionalno)
- [ ] Pregled povijesti mečeva igrača
- [ ] Osnovna statistika (pobjede/porazi)

**Rezultat:** Korisnici mogu evidentirati mečeve i unositi rezultate slobodnim tekstom.

---

### Faza 4 – Osnova za lige i turnire

**Cilj:** Mogućnost kreiranja liga, prijava sudionika i evidencija liga-mečeva.

**Zadaci:**
- [ ] Migracije baze: `leagues`, `league_participants`, `league_matches`
- [ ] CRUD endpointi za lige
- [ ] Prijava/odjava sudionika
- [ ] Vezanje mečeva uz ligu
- [ ] Obvezni unos rezultata za liga-mečeve
- [ ] Osnovna ljestvica (bodovi, pobjede, porazi)

**Rezultat:** Admin može kreirati ligu, dodati sudionike i pratiti rezultate.

---

### Faza 5 – Notifikacije (email + push)

**Cilj:** Automatske obavijesti za ključne događaje u sustavu.

**Zadaci:**
- [ ] Integracija email servisa (npr. Resend, SendGrid ili Supabase Edge Functions)
- [ ] Integracija push notifikacija (Firebase FCM ili Expo Push)
- [ ] Notifikacija pri potvrdi rezervacije
- [ ] Podsjetnik X sati prije termina (konfigurabilno)
- [ ] Notifikacija pri otkazivanju rezervacije
- [ ] Notifikacija pri odobrenju/odbijanju zahtjeva za članstvo
- [ ] Tablice i endpointi za upravljanje preferencijama notifikacija
- [ ] Mehanizam za raspoređivanje (background job / Supabase Edge Functions)

**Rezultat:** Korisnici primaju email i/ili push notifikacije za važne događaje.

---

### Faza 6 – Web frontend

**Cilj:** Korisnički sučelje dostupno putem preglednika.

**Zadaci:**
- [ ] Odabir frameworka (Blazor WASM ili React)
- [ ] Autentikacija (prijava, registracija, reset lozinke)
- [ ] Dashboard za igrača (moji tereni, moji mečevi, moje lige)
- [ ] Dashboard za admina (pregled kluba, upiti za članstvo)
- [ ] Kalendar rezervacija (pregled po terenu i datumu)
- [ ] Forma za rezervaciju termina
- [ ] Pregled i unos rezultata mečeva
- [ ] Pregled liga i ljestvice
- [ ] Profil korisnika

**Rezultat:** Funkcionalna web aplikacija za sve uloge.

---

### Faza 7 – Mobilne aplikacije (.NET MAUI)

**Cilj:** Cross-platform mobilne aplikacije (iOS + Android) s ključnim funkcionalnostima.

**Zadaci:**
- [ ] Inicijalizacija MAUI projekta
- [ ] Autentikacija (prijava, registracija)
- [ ] Pregled terena i kalendara (slobodni termini)
- [ ] Kreiranje i pregled rezervacija
- [ ] Pregled mečeva i unos rezultata
- [ ] Push notifikacije
- [ ] Profil korisnika

**Rezultat:** Mobilna aplikacija za igrače dostupna na iOS i Android platformama.

---

## 7. Struktura projekta

Projekt slijedi **Clean Architecture** princip s jasnim odvajanjem slojeva.

```
TennisManager/
│
├── TennisManager.sln
│
├── src/
│   │
│   ├── TennisManager.Domain/                  # Domenske klase i sučelja
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Club.cs
│   │   │   ├── ClubMember.cs
│   │   │   ├── Court.cs
│   │   │   ├── CourtSettings.cs
│   │   │   ├── Reservation.cs
│   │   │   ├── ReservationParticipant.cs
│   │   │   ├── Match.cs
│   │   │   ├── MatchPlayer.cs
│   │   │   ├── MatchResult.cs
│   │   │   ├── MatchSet.cs
│   │   │   ├── League.cs
│   │   │   ├── LeagueParticipant.cs
│   │   │   ├── LeagueMatch.cs
│   │   │   └── Notification.cs
│   │   ├── Enums/
│   │   │   ├── ClubRole.cs
│   │   │   ├── MemberStatus.cs
│   │   │   ├── CourtSurface.cs
│   │   │   ├── CourtEnvironment.cs
│   │   │   ├── ReservationType.cs
│   │   │   ├── ReservationStatus.cs
│   │   │   ├── MatchType.cs
│   │   │   ├── LeagueStatus.cs
│   │   │   ├── LeagueFormat.cs
│   │   │   ├── NotificationType.cs
│   │   │   ├── NotificationChannel.cs
│   │   │   └── NotificationStatus.cs
│   │   └── Interfaces/
│   │       ├── Repositories/
│   │       │   ├── IClubRepository.cs
│   │       │   ├── ICourtRepository.cs
│   │       │   ├── IReservationRepository.cs
│   │       │   ├── IMatchRepository.cs
│   │       │   └── ILeagueRepository.cs
│   │       └── Services/
│   │           ├── INotificationService.cs
│   │           └── IAuthService.cs
│   │
│   ├── TennisManager.Application/             # Poslovna logika (Use Cases)
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   └── LoggingBehavior.cs
│   │   │   ├── Exceptions/
│   │   │   │   ├── NotFoundException.cs
│   │   │   │   ├── ForbiddenException.cs
│   │   │   │   └── ValidationException.cs
│   │   │   └── Interfaces/
│   │   │       └── ICurrentUserService.cs
│   │   ├── Clubs/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateClub/
│   │   │   │   ├── UpdateClub/
│   │   │   │   └── UpdateClubSettings/
│   │   │   └── Queries/
│   │   │       ├── GetClubs/
│   │   │       └── GetClubById/
│   │   ├── Members/
│   │   │   ├── Commands/
│   │   │   │   ├── RequestMembership/
│   │   │   │   ├── ApproveMembership/
│   │   │   │   ├── RejectMembership/
│   │   │   │   └── UpdateMemberRole/
│   │   │   └── Queries/
│   │   │       ├── GetClubMembers/
│   │   │       └── GetPendingRequests/
│   │   ├── Courts/
│   │   ├── Reservations/
│   │   ├── Matches/
│   │   ├── Leagues/
│   │   └── Notifications/
│   │
│   ├── TennisManager.Infrastructure/          # Implementacije sučelja, EF Core, eksterni servisi
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/              # EF Core Fluent API konfiguracije
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── ClubConfiguration.cs
│   │   │   │   └── ...
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   │       ├── ClubRepository.cs
│   │   │       └── ...
│   │   ├── Auth/
│   │   │   └── SupabaseAuthService.cs
│   │   ├── Notifications/
│   │   │   ├── EmailNotificationService.cs
│   │   │   └── PushNotificationService.cs
│   │   └── Storage/
│   │       └── SupabaseStorageService.cs
│   │
│   └── TennisManager.API/                     # ASP.NET Core Web API
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── ClubsController.cs
│       │   ├── MembersController.cs
│       │   ├── CourtsController.cs
│       │   ├── ReservationsController.cs
│       │   ├── MatchesController.cs
│       │   ├── LeaguesController.cs
│       │   └── NotificationsController.cs
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── JwtMiddleware.cs
│       ├── Models/
│       │   └── ErrorResponse.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   ├── TennisManager.Domain.Tests/
│   ├── TennisManager.Application.Tests/
│   └── TennisManager.API.Tests/
│
└── clients/
    ├── TennisManager.Web/                     # Blazor WASM ili React
    └── TennisManager.Mobile/                  # .NET MAUI
```

---

## 8. Buduće nadogradnje

### 8.1 Naplata i članarine
- Integracija platnih sustava (Stripe, PayPal)
- Praćenje plaćanja i uplata članarina
- Automatski podsjetnici za dospjele članarine
- Generiranje računa/potvrda

### 8.2 Napredno rangiranje igrača (ELO sustav)
- Automatski izračun ELO bodova na temelju rezultata mečeva
- Globalna i klupska rang-ljestvica
- Usporedba igrača i napredak kroz sezone

### 8.3 Napredne statistike
- Statistike po terenu, sezoni, tipu podloge
- Vizualizacija podataka (grafovi, heatmape)
- Uvoz/izvoz podataka (CSV, Excel)
- Analitika za admina kluba (popunjenost terena, aktivnost članova)

### 8.4 Javne stranice kluba
- SEO-optimizirane javne stranice kluba
- Prikaz rasporeda mečeva i rezultata liga
- Online zahtjev za učlanjenje direktno s javne stranice

### 8.5 Napredno upravljanje ligama i turnirima
- Automatsko generiranje rasporeda (round-robin, eliminacije)
- Sustav žrijeba
- Grupna faza + play-off
- Vizualni prikaz rasporeda (bracket)

### 8.6 Integracija s vanjskim kalendarima
- Google Calendar / iCal sinkronizacija
- Dijeljenje termina i podsjetnici

### 8.7 Coach Portal
- Planiranje treninga i evidencija prisutnosti
- Praćenje napretka igrača
- Komunikacija trener–igrač

---

*Dokument je podložan izmjenama kako projekt napreduje.*
