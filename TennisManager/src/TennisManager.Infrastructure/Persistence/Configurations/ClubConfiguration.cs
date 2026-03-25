using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.ToTable("clubs");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description");

        builder.Property(c => c.Address)
            .HasColumnName("address")
            .HasMaxLength(300);

        builder.Property(c => c.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .HasColumnName("country")
            .HasMaxLength(2)
            .HasDefaultValue("HR")
            .IsRequired();

        builder.Property(c => c.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Club)
            .HasForeignKey(m => m.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Courts)
            .WithOne(ct => ct.Club)
            .HasForeignKey(ct => ct.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CourtSettings)
            .WithOne(cs => cs.Club)
            .HasForeignKey<CourtSettings>(cs => cs.ClubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
