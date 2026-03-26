using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.ToTable("leagues");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id");

        builder.Property(l => l.ClubId)
            .HasColumnName("club_id")
            .IsRequired();

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(l => l.Description)
            .HasColumnName("description");

        builder.Property(l => l.Format)
            .HasColumnName("format")
            .HasConversion<string>()
            .HasDefaultValue(TennisManager.Domain.Enums.LeagueFormat.RoundRobin)
            .IsRequired();

        builder.Property(l => l.MatchType)
            .HasColumnName("match_type")
            .HasConversion<string>()
            .HasDefaultValue(TennisManager.Domain.Enums.MatchType.Singles)
            .IsRequired();

        builder.Property(l => l.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(TennisManager.Domain.Enums.LeagueStatus.Draft)
            .IsRequired();

        builder.Property(l => l.StartsAt)
            .HasColumnName("starts_at");

        builder.Property(l => l.EndsAt)
            .HasColumnName("ends_at");

        builder.Property(l => l.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(l => l.ClubId)
            .HasDatabaseName("IX_leagues_club_id");

        builder.HasOne(l => l.Club)
            .WithMany()
            .HasForeignKey(l => l.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.CreatedByUser)
            .WithMany()
            .HasForeignKey(l => l.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
