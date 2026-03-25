using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface IClubMemberRepository
{
    Task<IEnumerable<ClubMember>> GetByClubIdAsync(Guid clubId);
    Task<IEnumerable<ClubMember>> GetPendingByClubIdAsync(Guid clubId);
    Task<ClubMember?> GetByClubAndUserAsync(Guid clubId, Guid userId);
    Task<ClubMember?> GetByIdAsync(Guid id);
    Task<ClubMember> CreateAsync(ClubMember member);
    Task UpdateAsync(ClubMember member);
    Task DeleteAsync(Guid clubId, Guid userId);
}
