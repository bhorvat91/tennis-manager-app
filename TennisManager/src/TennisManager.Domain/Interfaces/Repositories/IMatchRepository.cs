using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByClubIdAsync(Guid clubId);
    Task<IEnumerable<Match>> GetByUserIdAsync(Guid clubId, Guid userId);
    Task<Match> CreateAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(Guid id);
    Task AddPlayerAsync(MatchPlayer player);
    Task<MatchResult?> GetResultAsync(Guid matchId);
    Task<MatchResult> AddResultAsync(MatchResult result);
    Task UpdateResultAsync(MatchResult result);
    Task AddSetAsync(MatchSet set);
    Task<IEnumerable<MatchSet>> GetSetsAsync(Guid matchId);
    Task DeleteSetsAsync(Guid matchId);
    Task<int> GetWinCountAsync(Guid clubId, Guid userId);
    Task<int> GetLossCountAsync(Guid clubId, Guid userId);
    Task<int> GetTotalMatchCountAsync(Guid clubId, Guid userId);
}
