namespace TennisManager.Domain.Entities;

public class ReservationParticipant
{
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    public User User { get; set; } = null!;
}
