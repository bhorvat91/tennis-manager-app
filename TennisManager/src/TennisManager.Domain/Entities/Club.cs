namespace TennisManager.Domain.Entities;

public class Club
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string Country { get; set; } = "HR";
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ClubMember> Members { get; set; } = new List<ClubMember>();
    public ICollection<Court> Courts { get; set; } = new List<Court>();
    public CourtSettings? CourtSettings { get; set; }
}
