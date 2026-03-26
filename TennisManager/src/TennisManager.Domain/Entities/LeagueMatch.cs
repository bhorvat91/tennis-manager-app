namespace TennisManager.Domain.Entities;

public class LeagueMatch
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid MatchId { get; set; }
    public int? Round { get; set; }
    public string? GroupName { get; set; }
    public bool IsRequired { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public League League { get; set; } = null!;
    public Match Match { get; set; } = null!;
}
