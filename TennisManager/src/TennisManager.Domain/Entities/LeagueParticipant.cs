namespace TennisManager.Domain.Entities;

public class LeagueParticipant
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }
    public DateTime RegisteredAt { get; set; }

    public League League { get; set; } = null!;
    public User User { get; set; } = null!;
}
