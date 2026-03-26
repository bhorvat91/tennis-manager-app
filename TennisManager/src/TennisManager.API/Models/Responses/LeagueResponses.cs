using TennisManager.Domain.Enums;
using MatchType = TennisManager.Domain.Enums.MatchType;

namespace TennisManager.API.Models.Responses;

public class LeagueResponse
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LeagueFormat Format { get; set; }
    public MatchType MatchType { get; set; }
    public LeagueStatus Status { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ParticipantCount { get; set; }
}

public class LeagueParticipantResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class LeagueMatchResponse
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid MatchId { get; set; }
    public int? Round { get; set; }
    public string? GroupName { get; set; }
    public bool IsRequired { get; set; }
    public MatchSummaryResponse? Match { get; set; }
}

public class MatchSummaryResponse
{
    public Guid Id { get; set; }
    public MatchType MatchType { get; set; }
    public DateTime? PlayedAt { get; set; }
    public string? Notes { get; set; }
    public List<MatchPlayerResponse> Players { get; set; } = new();
    public int? WinnerTeam { get; set; }
}

public class StandingResponse
{
    public int Position { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int Points { get; set; }
    public int MatchesPlayed { get; set; }
}
