using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class LeagueMatchConfiguration : IEntityTypeConfiguration<LeagueMatch>
{
    public void Configure(EntityTypeBuilder<LeagueMatch> builder)
    {
        builder.ToTable("league_matches");

        builder.HasKey(lm => lm.Id);

        builder.Property(lm => lm.Id)
            .HasColumnName("id");

        builder.Property(lm => lm.LeagueId)
            .HasColumnName("league_id")
            .IsRequired();

        builder.Property(lm => lm.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(lm => lm.Round)
            .HasColumnName("round");

        builder.Property(lm => lm.GroupName)
            .HasColumnName("group_name");

        builder.Property(lm => lm.IsRequired)
            .HasColumnName("is_required")
            .HasDefaultValue(true);

        builder.Property(lm => lm.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(lm => lm.MatchId)
            .IsUnique()
            .HasDatabaseName("IX_league_matches_match_id");

        builder.HasOne(lm => lm.League)
            .WithMany(l => l.LeagueMatches)
            .HasForeignKey(lm => lm.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lm => lm.Match)
            .WithMany()
            .HasForeignKey(lm => lm.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
