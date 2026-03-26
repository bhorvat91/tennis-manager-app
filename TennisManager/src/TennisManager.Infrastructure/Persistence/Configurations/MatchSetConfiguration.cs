using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class MatchSetConfiguration : IEntityTypeConfiguration<MatchSet>
{
    public void Configure(EntityTypeBuilder<MatchSet> builder)
    {
        builder.ToTable("match_sets");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(s => s.SetNumber)
            .HasColumnName("set_number")
            .IsRequired();

        builder.Property(s => s.Team1Games)
            .HasColumnName("team1_games");

        builder.Property(s => s.Team2Games)
            .HasColumnName("team2_games");

        builder.HasAlternateKey(s => new { s.MatchId, s.SetNumber });

        builder.HasOne(s => s.Match)
            .WithMany(m => m.Sets)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
