using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class CourtRepository : ICourtRepository
{
    private readonly AppDbContext _db;

    public CourtRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Court>> GetByClubIdAsync(Guid clubId)
    {
        return await _db.Courts
            .Where(c => c.ClubId == clubId && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Court?> GetByIdAsync(Guid id)
    {
        return await _db.Courts.FindAsync(id);
    }

    public async Task<Court> CreateAsync(Court court)
    {
        court.Id = Guid.NewGuid();
        court.CreatedAt = DateTime.UtcNow;
        court.UpdatedAt = DateTime.UtcNow;
        _db.Courts.Add(court);
        await _db.SaveChangesAsync();
        return court;
    }

    public async Task UpdateAsync(Court court)
    {
        court.UpdatedAt = DateTime.UtcNow;
        _db.Courts.Update(court);
        await _db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var court = await _db.Courts.FindAsync(id);
        if (court is not null)
        {
            court.IsActive = false;
            court.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
