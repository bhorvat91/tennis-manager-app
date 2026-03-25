using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface ICourtSettingsRepository
{
    Task<CourtSettings?> GetByClubIdAsync(Guid clubId);
    Task<CourtSettings> CreateAsync(CourtSettings settings);
    Task UpdateAsync(CourtSettings settings);
}
