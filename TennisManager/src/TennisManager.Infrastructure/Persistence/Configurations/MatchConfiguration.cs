using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("matches");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.ClubId)
            .HasColumnName("club_id")
            .IsRequired();

        builder.Property(m => m.ReservationId)
            .HasColumnName("reservation_id");

        builder.Property(m => m.LeagueId)
            .HasColumnName("league_id");

        builder.Property(m => m.MatchType)
            .HasColumnName("match_type")
            .HasConversion<string>()
            .HasDefaultValue(TennisManager.Domain.Enums.MatchType.Singles)
            .IsRequired();

        builder.Property(m => m.PlayedAt)
            .HasColumnName("played_at");

        builder.Property(m => m.Notes)
            .HasColumnName("notes");

        builder.Property(m => m.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(m => new { m.ClubId, m.PlayedAt })
            .HasDatabaseName("IX_matches_club_played_at");

        builder.HasOne(m => m.Club)
            .WithMany()
            .HasForeignKey(m => m.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Reservation)
            .WithMany()
            .HasForeignKey(m => m.ReservationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Creator)
            .WithMany()
            .HasForeignKey(m => m.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
