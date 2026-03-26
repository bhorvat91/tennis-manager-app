using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class ReservationParticipantConfiguration : IEntityTypeConfiguration<ReservationParticipant>
{
    public void Configure(EntityTypeBuilder<ReservationParticipant> builder)
    {
        builder.ToTable("reservation_participants");

        builder.HasKey(rp => new { rp.ReservationId, rp.UserId });

        builder.Property(rp => rp.ReservationId)
            .HasColumnName("reservation_id");

        builder.Property(rp => rp.UserId)
            .HasColumnName("user_id");

        builder.HasOne(rp => rp.Reservation)
            .WithMany(r => r.Participants)
            .HasForeignKey(rp => rp.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.User)
            .WithMany()
            .HasForeignKey(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
