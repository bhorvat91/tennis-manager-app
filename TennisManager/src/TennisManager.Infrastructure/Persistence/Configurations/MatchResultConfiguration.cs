using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisManager.Domain.Entities;

namespace TennisManager.Infrastructure.Persistence.Configurations;

public class MatchResultConfiguration : IEntityTypeConfiguration<MatchResult>
{
    public void Configure(EntityTypeBuilder<MatchResult> builder)
    {
        builder.ToTable("match_results");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(r => r.WinnerTeam)
            .HasColumnName("winner_team");

        builder.Property(r => r.ResultText)
            .HasColumnName("result_text");

        builder.Property(r => r.EnteredBy)
            .HasColumnName("entered_by")
            .IsRequired();

        builder.Property(r => r.EnteredAt)
            .HasColumnName("entered_at")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(r => r.MatchId)
            .IsUnique()
            .HasDatabaseName("IX_match_results_match_id");

        builder.HasOne(r => r.Match)
            .WithOne(m => m.Result)
            .HasForeignKey<MatchResult>(r => r.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.EnteredByUser)
            .WithMany()
            .HasForeignKey(r => r.EnteredBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
