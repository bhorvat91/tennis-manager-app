namespace TennisManager.Domain.Entities;

public class CourtSettings
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public int MinReservationMinutes { get; set; } = 30;
    public int MaxReservationMinutes { get; set; } = 120;
    public int CancellationDeadlineHours { get; set; } = 24;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Club Club { get; set; } = null!;
}
