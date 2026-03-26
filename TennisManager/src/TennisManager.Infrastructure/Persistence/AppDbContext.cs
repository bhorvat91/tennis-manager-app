using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<ClubMember> ClubMembers => Set<ClubMember>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<CourtSettings> CourtSettings => Set<CourtSettings>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationParticipant> ReservationParticipants => Set<ReservationParticipant>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchPlayer> MatchPlayers => Set<MatchPlayer>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();
    public DbSet<MatchSet> MatchSets => Set<MatchSet>();
    public DbSet<League> Leagues => Set<League>();
    public DbSet<LeagueParticipant> LeagueParticipants => Set<LeagueParticipant>();
    public DbSet<LeagueMatch> LeagueMatches => Set<LeagueMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
