using TennisManager.Domain.Enums;

namespace TennisManager.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid CourtId { get; set; }
    public Guid CreatedBy { get; set; }
    public ReservationType ReservationType { get; set; } = ReservationType.Other;
    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string? Notes { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? CancelledBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Club Club { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public User? Canceller { get; set; }
    public ICollection<ReservationParticipant> Participants { get; set; } = new List<ReservationParticipant>();
}
