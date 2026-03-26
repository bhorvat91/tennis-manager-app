namespace TennisManager.Web.Models;

public class LeagueDto
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Format { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int ParticipantCount { get; set; }
}

public class CreateLeagueRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateLeagueRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Format { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class LeagueParticipantDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class LeagueMatchDto
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public Guid MatchId { get; set; }
    public string Round { get; set; } = string.Empty;
    public MatchDto? Match { get; set; }
}

public class StandingDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalMatches { get; set; }
}
