namespace TennisManager.Mobile.Models;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateReservationRequest
{
    public Guid CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
}

public class CancelReservationRequest
{
    public string? Reason { get; set; }
}

public class AvailabilitySlot
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }

    public string TimeLabel => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    public string StatusLabel => IsAvailable ? "Slobodno" : "Zauzeto";
    public Color StatusColor => IsAvailable ? Colors.Green : Colors.Red;
}
