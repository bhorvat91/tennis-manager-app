using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class CourtSettingsConfiguration : IEntityTypeConfiguration<CourtSettings>
{
    public void Configure(EntityTypeBuilder<CourtSettings> builder)
    {
        builder.ToTable("court_settings");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Id)
            .HasColumnName("id");

        builder.Property(cs => cs.ClubId)
            .HasColumnName("club_id")
            .IsRequired();

        builder.HasIndex(cs => cs.ClubId)
            .IsUnique();

        builder.Property(cs => cs.MinReservationMinutes)
            .HasColumnName("min_reservation_minutes")
            .HasDefaultValue(30);

        builder.Property(cs => cs.MaxReservationMinutes)
            .HasColumnName("max_reservation_minutes")
            .HasDefaultValue(120);

        builder.Property(cs => cs.CancellationDeadlineHours)
            .HasColumnName("cancellation_deadline_hours")
            .HasDefaultValue(24);

        builder.Property(cs => cs.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(cs => cs.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");
    }
}
