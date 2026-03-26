using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.ClubId)
            .HasColumnName("club_id")
            .IsRequired();

        builder.Property(r => r.CourtId)
            .HasColumnName("court_id")
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(r => r.ReservationType)
            .HasColumnName("reservation_type")
            .HasConversion<string>()
            .HasDefaultValue(ReservationType.Other)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(ReservationStatus.Confirmed)
            .IsRequired();

        builder.Property(r => r.StartsAt)
            .HasColumnName("starts_at")
            .IsRequired();

        builder.Property(r => r.EndsAt)
            .HasColumnName("ends_at")
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasColumnName("notes");

        builder.Property(r => r.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(r => r.CancelledBy)
            .HasColumnName("cancelled_by");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        builder.ToTable(t => t.HasCheckConstraint("CK_reservations_ends_after_starts", "ends_at > starts_at"));

        builder.HasIndex(r => new { r.CourtId, r.StartsAt, r.EndsAt })
            .HasDatabaseName("IX_reservations_court_time");

        builder.HasOne(r => r.Club)
            .WithMany()
            .HasForeignKey(r => r.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Court)
            .WithMany()
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Creator)
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Canceller)
            .WithMany()
            .HasForeignKey(r => r.CancelledBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
