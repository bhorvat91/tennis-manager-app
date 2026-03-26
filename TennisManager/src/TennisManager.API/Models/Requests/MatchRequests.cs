using MatchType = TennisManager.Domain.Enums.MatchType;

namespace TennisManager.API.Models.Requests;

public class CreateMatchRequest
{
    public Guid? ReservationId { get; set; }
    public MatchType MatchType { get; set; } = MatchType.Singles;
    public DateTime? PlayedAt { get; set; }
    public string? Notes { get; set; }
    public List<MatchPlayerRequest> Players { get; set; } = new();
}

public class MatchPlayerRequest
{
    public Guid UserId { get; set; }
    public int Team { get; set; }
}

public class UpdateMatchRequest
{
    public MatchType? MatchType { get; set; }
    public DateTime? PlayedAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateMatchResultRequest
{
    public int? WinnerTeam { get; set; }
    public string? ResultText { get; set; }
    public List<MatchSetRequest>? Sets { get; set; }
}

public class MatchSetRequest
{
    public int SetNumber { get; set; }
    public int? Team1Games { get; set; }
    public int? Team2Games { get; set; }
}

public class UpdateMatchResultRequest
{
    public int? WinnerTeam { get; set; }
    public string? ResultText { get; set; }
    public List<MatchSetRequest>? Sets { get; set; }
}
