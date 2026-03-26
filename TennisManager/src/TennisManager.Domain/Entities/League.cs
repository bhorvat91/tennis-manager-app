using TennisManager.Domain.Enums;

namespace TennisManager.Domain.Entities;

public class League
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LeagueFormat Format { get; set; } = LeagueFormat.RoundRobin;
    public Enums.MatchType MatchType { get; set; } = Enums.MatchType.Singles;
    public LeagueStatus Status { get; set; } = LeagueStatus.Draft;
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Club Club { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<LeagueParticipant> Participants { get; set; } = new List<LeagueParticipant>();
    public ICollection<LeagueMatch> LeagueMatches { get; set; } = new List<LeagueMatch>();
}
