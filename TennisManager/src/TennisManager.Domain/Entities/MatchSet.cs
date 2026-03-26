namespace TennisManager.Domain.Entities;

public class MatchSet
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public int SetNumber { get; set; }
    public int? Team1Games { get; set; }
    public int? Team2Games { get; set; }

    public Match Match { get; set; } = null!;
}
