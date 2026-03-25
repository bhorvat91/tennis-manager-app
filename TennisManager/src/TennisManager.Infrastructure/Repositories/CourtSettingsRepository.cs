using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class CourtSettingsRepository : ICourtSettingsRepository
{
    private readonly AppDbContext _db;

    public CourtSettingsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CourtSettings?> GetByClubIdAsync(Guid clubId)
    {
        return await _db.CourtSettings.FirstOrDefaultAsync(cs => cs.ClubId == clubId);
    }

    public async Task<CourtSettings> CreateAsync(CourtSettings settings)
    {
        settings.Id = Guid.NewGuid();
        settings.CreatedAt = DateTime.UtcNow;
        settings.UpdatedAt = DateTime.UtcNow;
        _db.CourtSettings.Add(settings);
        await _db.SaveChangesAsync();
        return settings;
    }

    public async Task UpdateAsync(CourtSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _db.CourtSettings.Update(settings);
        await _db.SaveChangesAsync();
    }
}
