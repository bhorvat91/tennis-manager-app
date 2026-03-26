# 🎾 Tennis Manager App

## O projektu

**Tennis Manager App** je multi-tenant aplikacija za upravljanje tenis klubovima. Jedan sustav podržava više nezavisnih klubova, a svaki igrač može biti član više klubova istovremeno.

Aplikacija omogućava upravljanje rezervacijama terena, članovima, mečevima, rezultatima i ligama unutar klubova – dostupna putem weba i mobilnih uređaja.

---

## Tehnološki stog

| Sloj | Tehnologija |
|------|------------|
| Backend API | ASP.NET Core Web API (.NET 8+) |
| Baza podataka | Supabase (PostgreSQL) |
| ORM | Entity Framework Core + Npgsql |
| Web frontend | Blazor WebAssembly |
| Mobilne aplikacije | .NET MAUI |
| Autentikacija | Supabase Auth + JWT |
| Pohrana datoteka | Supabase Storage |
| Notifikacije | Email + Push |

---

## Funkcionalnosti

- Multi-tenant sustav (više klubova)
- Upravljanje članovima i ulogama (Admin, Trener, Igrač, Gost)
- Upravljanje terenima
- Sustav rezervacija s konfigurabilnim pravilima
- Evidencija mečeva i rezultata (singl i parovi)
- Lige i turniri
- Notifikacije (email + push)

---

## Struktura projekta

Za detaljan pregled strukture projekta i Clean Architecture pristupa, pogledaj [implementation.md](implementation.md).

---

## Pokretanje projekta

### Preduvjeti

- .NET 8+ SDK
- Supabase account (free tier)
- Visual Studio 2022 / VS Code / JetBrains Rider

### Postavljanje

1. Klonirati repozitorij
   ```bash
   git clone https://github.com/bhorvat91/tennis-manager-app.git
   cd tennis-manager-app
   ```

2. Postaviti Supabase projekt na [supabase.com](https://supabase.com)

3. Konfigurirati connection string u `appsettings.json`
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
     }
   }
   ```

4. Instalirati ovisnosti
   ```bash
   dotnet restore
   ```

5. Pokrenuti migracije
   ```bash
   dotnet ef database update
   ```

6. Pokrenuti API
   ```bash
   dotnet run
   ```

---

## Dokumentacija

- 📄 [implementation.md](implementation.md) – Detaljan implementacijski plan s fazama razvoja, shemom baze podataka, API endpointima i strukturom projekta

---

## 📋 Praćenje napretka (Feature Tracker)

> Legenda: ⬜ Nije započeto &nbsp; 🔄 U tijeku &nbsp; ✅ Završeno

### Faza 1 – MVP: Autentikacija + Klubovi + Članovi + Tereni

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ✅ | Postavljanje Supabase projekta | 2026-03-25 | PR #3, #4 |
| ✅ | Inicijalizacija ASP.NET Core API projekta | 2026-03-25 | PR #3, #4 |
| ✅ | Supabase Auth + JWT integracija | 2026-03-25 | PR #3, #4 |
| ✅ | EF Core migracije (users, clubs, club_members, courts, court_settings) | 2026-03-25 | PR #3, #4 |
| ✅ | CRUD endpointi za klubove | 2026-03-25 | PR #3, #4 |
| ✅ | Upravljanje članovima (zahtjev, odobravanje, uloge) | 2026-03-25 | PR #3, #4 |
| ✅ | CRUD endpointi za terene | 2026-03-25 | PR #3, #4 |
| ✅ | Postavke kluba (trajanje termina, rok otkazivanja) | 2026-03-25 | PR #3, #4 |
| ✅ | Autorizacija po ulogama | 2026-03-25 | PR #3, #4 |

### Faza 2 – Sustav rezervacija

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ✅ | Migracije: reservations, reservation_participants | 2026-03-26 | PR #5 |
| ✅ | CRUD endpointi za rezervacije | 2026-03-26 | PR #5 |
| ✅ | Validacija rezervacija (trajanje, preklapanje) | 2026-03-26 | PR #5 |
| ✅ | Rok otkazivanja | 2026-03-26 | PR #5 |
| ✅ | Provjera dostupnosti terena | 2026-03-26 | PR #5 |

### Faza 3 – Mečevi i rezultati

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ✅ | Migracije: matches, match_players, match_results, match_sets | 2026-03-26 | PR #6 |
| ✅ | CRUD endpointi za mečeve | 2026-03-26 | PR #6 |
| ✅ | Unos i ažuriranje rezultata | 2026-03-26 | PR #6 |
| ✅ | Vezanje meča uz rezervaciju | 2026-03-26 | PR #6 |
| ✅ | Povijest mečeva i statistika | 2026-03-26 | PR #6 |

### Faza 4 – Lige i turniri

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ⬜ | Migracije: leagues, league_participants, league_matches | - | - |
| ⬜ | CRUD endpointi za lige | - | - |
| ⬜ | Prijava/odjava sudionika | - | - |
| ⬜ | Liga-mečevi i obvezni rezultati | - | - |
| ⬜ | Osnovna ljestvica | - | - |

### Faza 5 – Notifikacije

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ⬜ | Email servis integracija | - | - |
| ⬜ | Push notifikacije | - | - |
| ⬜ | Potvrda rezervacije | - | - |
| ⬜ | Podsjetnik prije termina | - | - |
| ⬜ | Notifikacija za članstvo | - | - |

### Faza 6 – Web frontend (Blazor WebAssembly)

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ⬜ | Blazor WASM projekt setup | - | - |
| ⬜ | Autentikacija (login, registracija) | - | - |
| ⬜ | Dashboard igrača | - | - |
| ⬜ | Dashboard admina | - | - |
| ⬜ | Kalendar rezervacija | - | - |
| ⬜ | Pregled mečeva i rezultata | - | - |
| ⬜ | Pregled liga | - | - |

### Faza 7 – Mobilne aplikacije (.NET MAUI)

| Status | Feature | Datum završetka | Napomene |
|--------|---------|-----------------|----------|
| ⬜ | MAUI projekt setup | - | - |
| ⬜ | Autentikacija | - | - |
| ⬜ | Pregled i rezervacija terena | - | - |
| ⬜ | Mečevi i rezultati | - | - |
| ⬜ | Push notifikacije | - | - |

---

## Licenca

TBD
