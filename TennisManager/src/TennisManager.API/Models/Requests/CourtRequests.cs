using System.ComponentModel.DataAnnotations;
using TennisManager.Domain.Enums;

namespace TennisManager.API.Models.Requests;

public class CreateCourtRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CourtSurface Surface { get; set; }

    [Required]
    public CourtEnvironment Environment { get; set; }
}

public class UpdateCourtRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }

    public CourtSurface? Surface { get; set; }

    public CourtEnvironment? Environment { get; set; }
}
