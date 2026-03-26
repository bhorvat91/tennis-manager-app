namespace TennisManager.Domain.Entities;

public class MatchResult
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public int? WinnerTeam { get; set; }
    public string? ResultText { get; set; }
    public Guid EnteredBy { get; set; }
    public DateTime EnteredAt { get; set; }

    public Match Match { get; set; } = null!;
    public User EnteredByUser { get; set; } = null!;
}
