using TennisManager.Domain.Enums;

namespace TennisManager.API.Models.Responses;

public class CourtResponse
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CourtSurface Surface { get; set; }
    public CourtEnvironment Environment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
