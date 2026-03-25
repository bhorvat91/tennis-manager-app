using TennisManager.Domain.Enums;

namespace TennisManager.API.Models.Requests;

public class CreateReservationRequest
{
    public Guid CourtId { get; set; }
    public ReservationType ReservationType { get; set; } = ReservationType.Other;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string? Notes { get; set; }
    public List<Guid>? ParticipantUserIds { get; set; }
}

public class UpdateReservationRequest
{
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public string? Notes { get; set; }
    public ReservationType? ReservationType { get; set; }
}
