namespace TennisManager.API.Models.Responses;

public class ClubResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CourtSettingsResponse
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public int MinReservationMinutes { get; set; }
    public int MaxReservationMinutes { get; set; }
    public int CancellationDeadlineHours { get; set; }
}
