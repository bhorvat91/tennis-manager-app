using TennisManager.Domain.Enums;

namespace TennisManager.Domain.Entities;

public class ClubMember
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid UserId { get; set; }
    public ClubRole Role { get; set; } = ClubRole.Player;
    public MemberStatus Status { get; set; } = MemberStatus.Pending;
    public DateTime? JoinedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Club Club { get; set; } = null!;
    public User User { get; set; } = null!;
}
