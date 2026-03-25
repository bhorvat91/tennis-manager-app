using System.ComponentModel.DataAnnotations;

namespace TennisManager.API.Models.Requests;

public class CreateClubRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(2)]
    public string Country { get; set; } = "HR";

    [MaxLength(500)]
    public string? LogoUrl { get; set; }
}

public class UpdateClubRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(2)]
    public string? Country { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }
}

public class UpdateCourtSettingsRequest
{
    public const int MinAllowedReservationMinutes = 30;

    [Range(MinAllowedReservationMinutes, int.MaxValue, ErrorMessage = "MinReservationMinutes must be at least 30.")]
    public int MinReservationMinutes { get; set; } = MinAllowedReservationMinutes;

    [Range(MinAllowedReservationMinutes, int.MaxValue)]
    public int MaxReservationMinutes { get; set; } = 120;

    [Range(0, int.MaxValue)]
    public int CancellationDeadlineHours { get; set; } = 24;
}
