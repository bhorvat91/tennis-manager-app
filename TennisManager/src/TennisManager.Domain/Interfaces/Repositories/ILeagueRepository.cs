using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public class LeagueStanding
{
    public Guid UserId { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int Points { get; set; }
    public int MatchesPlayed { get; set; }
}

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    Task<IEnumerable<League>> GetByClubIdAsync(Guid clubId);
    Task<League> CreateAsync(League league);
    Task UpdateAsync(League league);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<LeagueParticipant>> GetParticipantsAsync(Guid leagueId);
    Task AddParticipantAsync(LeagueParticipant participant);
    Task RemoveParticipantAsync(Guid leagueId, Guid userId);
    Task<bool> IsParticipantAsync(Guid leagueId, Guid userId);
    Task<IEnumerable<LeagueMatch>> GetLeagueMatchesAsync(Guid leagueId);
    Task AddLeagueMatchAsync(LeagueMatch leagueMatch);
    Task<IEnumerable<LeagueStanding>> GetStandingsAsync(Guid leagueId);
}
