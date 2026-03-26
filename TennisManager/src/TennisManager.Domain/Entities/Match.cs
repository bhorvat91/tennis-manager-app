using TennisManager.Domain.Enums;

namespace TennisManager.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid? ReservationId { get; set; }
    public Guid? LeagueId { get; set; }
    public Enums.MatchType MatchType { get; set; } = Enums.MatchType.Singles;
    public DateTime? PlayedAt { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Club Club { get; set; } = null!;
    public Reservation? Reservation { get; set; }
    public User Creator { get; set; } = null!;
    public ICollection<MatchPlayer> Players { get; set; } = new List<MatchPlayer>();
    public MatchResult? Result { get; set; }
    public ICollection<MatchSet> Sets { get; set; } = new List<MatchSet>();
}
