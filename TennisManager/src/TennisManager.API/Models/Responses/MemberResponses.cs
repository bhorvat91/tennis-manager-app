using TennisManager.Domain.Enums;

namespace TennisManager.API.Models.Responses;

public class ClubMemberResponse
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public ClubRole Role { get; set; }
    public MemberStatus Status { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
