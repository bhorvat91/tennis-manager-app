using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface ICourtRepository
{
    Task<IEnumerable<Court>> GetByClubIdAsync(Guid clubId);
    Task<Court?> GetByIdAsync(Guid id);
    Task<Court> CreateAsync(Court court);
    Task UpdateAsync(Court court);
    Task DeactivateAsync(Guid id);
}
