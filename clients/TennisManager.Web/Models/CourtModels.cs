namespace TennisManager.Web.Models;

public class CourtDto
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surface { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateCourtRequest
{
    public string Name { get; set; } = string.Empty;
    public string Surface { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
}

public class UpdateCourtRequest
{
    public string Name { get; set; } = string.Empty;
    public string Surface { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
