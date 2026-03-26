using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class MatchPlayerConfiguration : IEntityTypeConfiguration<MatchPlayer>
{
    public void Configure(EntityTypeBuilder<MatchPlayer> builder)
    {
        builder.ToTable("match_players");

        builder.HasKey(mp => new { mp.MatchId, mp.UserId });

        builder.Property(mp => mp.MatchId)
            .HasColumnName("match_id");

        builder.Property(mp => mp.UserId)
            .HasColumnName("user_id");

        builder.Property(mp => mp.Team)
            .HasColumnName("team")
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_match_players_team", "team = 1 OR team = 2"));

        builder.HasIndex(mp => mp.UserId)
            .HasDatabaseName("IX_match_players_user_id");

        builder.HasOne(mp => mp.Match)
            .WithMany(m => m.Players)
            .HasForeignKey(mp => mp.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mp => mp.User)
            .WithMany()
            .HasForeignKey(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
