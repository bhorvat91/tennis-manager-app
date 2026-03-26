namespace TennisManager.Domain.Entities;

public class MatchPlayer
{
    public Guid MatchId { get; set; }
    public Guid UserId { get; set; }
    public int Team { get; set; }

    public Match Match { get; set; } = null!;
    public User User { get; set; } = null!;
}
