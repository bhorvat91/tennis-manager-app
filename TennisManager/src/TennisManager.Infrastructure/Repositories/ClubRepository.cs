using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class ClubRepository : IClubRepository
{
    private readonly AppDbContext _db;

    public ClubRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Club>> GetAllActiveAsync()
    {
        return await _db.Clubs
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Club?> GetByIdAsync(Guid id)
    {
        return await _db.Clubs
            .Include(c => c.CourtSettings)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Club> CreateAsync(Club club)
    {
        club.Id = Guid.NewGuid();
        club.CreatedAt = DateTime.UtcNow;
        club.UpdatedAt = DateTime.UtcNow;
        _db.Clubs.Add(club);
        await _db.SaveChangesAsync();
        return club;
    }

    public async Task UpdateAsync(Club club)
    {
        club.UpdatedAt = DateTime.UtcNow;
        _db.Clubs.Update(club);
        await _db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var club = await _db.Clubs.FindAsync(id);
        if (club is not null)
        {
            club.IsActive = false;
            club.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
