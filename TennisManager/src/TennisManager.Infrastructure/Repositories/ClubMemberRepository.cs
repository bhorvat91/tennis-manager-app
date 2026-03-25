using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class ClubMemberRepository : IClubMemberRepository
{
    private readonly AppDbContext _db;

    public ClubMemberRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ClubMember>> GetByClubIdAsync(Guid clubId)
    {
        return await _db.ClubMembers
            .Include(m => m.User)
            .Where(m => m.ClubId == clubId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClubMember>> GetPendingByClubIdAsync(Guid clubId)
    {
        return await _db.ClubMembers
            .Include(m => m.User)
            .Where(m => m.ClubId == clubId && m.Status == Domain.Enums.MemberStatus.Pending)
            .ToListAsync();
    }

    public async Task<ClubMember?> GetByClubAndUserAsync(Guid clubId, Guid userId)
    {
        return await _db.ClubMembers
            .FirstOrDefaultAsync(m => m.ClubId == clubId && m.UserId == userId);
    }

    public async Task<ClubMember?> GetByIdAsync(Guid id)
    {
        return await _db.ClubMembers
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<ClubMember> CreateAsync(ClubMember member)
    {
        member.Id = Guid.NewGuid();
        member.CreatedAt = DateTime.UtcNow;
        member.UpdatedAt = DateTime.UtcNow;
        _db.ClubMembers.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(ClubMember member)
    {
        member.UpdatedAt = DateTime.UtcNow;
        _db.ClubMembers.Update(member);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid clubId, Guid userId)
    {
        var member = await _db.ClubMembers
            .FirstOrDefaultAsync(m => m.ClubId == clubId && m.UserId == userId);
        if (member is not null)
        {
            _db.ClubMembers.Remove(member);
            await _db.SaveChangesAsync();
        }
    }
}
