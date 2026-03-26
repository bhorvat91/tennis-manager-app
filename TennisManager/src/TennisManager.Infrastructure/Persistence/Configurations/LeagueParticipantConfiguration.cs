using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class LeagueParticipantConfiguration : IEntityTypeConfiguration<LeagueParticipant>
{
    public void Configure(EntityTypeBuilder<LeagueParticipant> builder)
    {
        builder.ToTable("league_participants");

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Id)
            .HasColumnName("id");

        builder.Property(lp => lp.LeagueId)
            .HasColumnName("league_id")
            .IsRequired();

        builder.Property(lp => lp.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(lp => lp.RegisteredAt)
            .HasColumnName("registered_at")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(lp => new { lp.LeagueId, lp.UserId })
            .IsUnique()
            .HasDatabaseName("IX_league_participants_league_user");

        builder.HasOne(lp => lp.League)
            .WithMany(l => l.Participants)
            .HasForeignKey(lp => lp.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lp => lp.User)
            .WithMany()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
