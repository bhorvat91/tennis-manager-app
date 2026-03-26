using MatchType = TennisManager.Domain.Enums.MatchType;

namespace TennisManager.API.Models.Responses;

public class MatchResponse
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid? ReservationId { get; set; }
    public MatchType MatchType { get; set; }
    public DateTime? PlayedAt { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<MatchPlayerResponse> Players { get; set; } = new();
    public MatchResultResponse? Result { get; set; }
}

public class MatchPlayerResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Team { get; set; }
}

public class MatchResultResponse
{
    public Guid Id { get; set; }
    public int? WinnerTeam { get; set; }
    public string? ResultText { get; set; }
    public Guid EnteredBy { get; set; }
    public DateTime EnteredAt { get; set; }
    public List<MatchSetResponse>? Sets { get; set; }
}

public class MatchSetResponse
{
    public int SetNumber { get; set; }
    public int? Team1Games { get; set; }
    public int? Team2Games { get; set; }
}

public class PlayerStatsResponse
{
    public Guid UserId { get; set; }
    public int TotalMatches { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
}
