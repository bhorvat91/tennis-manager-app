namespace TennisManager.Mobile.Models;

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

public class StandingDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalMatches { get; set; }
}
