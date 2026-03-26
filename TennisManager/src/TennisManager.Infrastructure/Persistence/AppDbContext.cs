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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
