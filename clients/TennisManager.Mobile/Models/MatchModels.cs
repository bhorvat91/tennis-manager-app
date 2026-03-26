namespace TennisManager.Mobile.Models;

public class MatchDto
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public Guid? ReservationId { get; set; }
    public List<string> Team1Players { get; set; } = new();
    public List<string> Team2Players { get; set; } = new();
    public MatchResultDto? Result { get; set; }

    public string Team1Display => string.Join(", ", Team1Players);
    public string Team2Display => string.Join(", ", Team2Players);
}

public class CreateMatchRequest
{
    public string Type { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public Guid? ReservationId { get; set; }
    public List<Guid> Team1PlayerIds { get; set; } = new();
    public List<Guid> Team2PlayerIds { get; set; } = new();
}

public class MatchResultDto
{
    public Guid WinnerId { get; set; }
    public string Score { get; set; } = string.Empty;
    public List<MatchSetDto> Sets { get; set; } = new();
}

public class MatchSetDto
{
    public int SetNumber { get; set; }
    public int Team1Games { get; set; }
    public int Team2Games { get; set; }

    public string Display => $"{Team1Games}:{Team2Games}";
}

public class EnterResultRequest
{
    public Guid WinnerId { get; set; }
    public List<MatchSetDto> Sets { get; set; } = new();
}

public class PlayerStatsDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int TotalMatches { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
}
