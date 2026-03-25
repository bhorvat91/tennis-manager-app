using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface IClubRepository
{
    Task<IEnumerable<Club>> GetAllActiveAsync();
    Task<Club?> GetByIdAsync(Guid id);
    Task<Club> CreateAsync(Club club);
    Task UpdateAsync(Club club);
    Task DeactivateAsync(Guid id);
}
