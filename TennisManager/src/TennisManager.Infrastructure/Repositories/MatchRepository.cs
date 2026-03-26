using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly AppDbContext _db;

    public MatchRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _db.Matches
            .Include(m => m.Club)
            .Include(m => m.Creator)
            .Include(m => m.Players)
                .ThenInclude(p => p.User)
            .Include(m => m.Result)
                .ThenInclude(r => r!.EnteredByUser)
            .Include(m => m.Sets)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetByClubIdAsync(Guid clubId)
    {
        return await _db.Matches
            .Include(m => m.Club)
            .Include(m => m.Creator)
            .Include(m => m.Players)
                .ThenInclude(p => p.User)
            .Include(m => m.Result)
            .Include(m => m.Sets)
            .Where(m => m.ClubId == clubId)
            .OrderByDescending(m => m.PlayedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByUserIdAsync(Guid clubId, Guid userId)
    {
        return await _db.Matches
            .Include(m => m.Club)
            .Include(m => m.Creator)
            .Include(m => m.Players)
                .ThenInclude(p => p.User)
            .Include(m => m.Result)
            .Include(m => m.Sets)
            .Where(m => m.ClubId == clubId && m.Players.Any(p => p.UserId == userId))
            .OrderByDescending(m => m.PlayedAt)
            .ToListAsync();
    }

    public async Task<Match> CreateAsync(Match match)
    {
        match.Id = Guid.NewGuid();
        match.CreatedAt = DateTime.UtcNow;
        match.UpdatedAt = DateTime.UtcNow;
        _db.Matches.Add(match);
        await _db.SaveChangesAsync();
        return match;
    }

    public async Task UpdateAsync(Match match)
    {
        match.UpdatedAt = DateTime.UtcNow;
        _db.Matches.Update(match);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match is not null)
        {
            _db.Matches.Remove(match);
            await _db.SaveChangesAsync();
        }
    }

    public async Task AddPlayerAsync(MatchPlayer player)
    {
        _db.MatchPlayers.Add(player);
        await _db.SaveChangesAsync();
    }

    public async Task<MatchResult?> GetResultAsync(Guid matchId)
    {
        return await _db.MatchResults
            .Include(r => r.EnteredByUser)
            .FirstOrDefaultAsync(r => r.MatchId == matchId);
    }

    public async Task<MatchResult> AddResultAsync(MatchResult result)
    {
        result.Id = Guid.NewGuid();
        result.EnteredAt = DateTime.UtcNow;
        _db.MatchResults.Add(result);
        await _db.SaveChangesAsync();
        return result;
    }

    public async Task UpdateResultAsync(MatchResult result)
    {
        _db.MatchResults.Update(result);
        await _db.SaveChangesAsync();
    }

    public async Task AddSetAsync(MatchSet set)
    {
        set.Id = Guid.NewGuid();
        _db.MatchSets.Add(set);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<MatchSet>> GetSetsAsync(Guid matchId)
    {
        return await _db.MatchSets
            .Where(s => s.MatchId == matchId)
            .OrderBy(s => s.SetNumber)
            .ToListAsync();
    }

    public async Task DeleteSetsAsync(Guid matchId)
    {
        var sets = await _db.MatchSets
            .Where(s => s.MatchId == matchId)
            .ToListAsync();
        _db.MatchSets.RemoveRange(sets);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetWinCountAsync(Guid clubId, Guid userId)
    {
        return await _db.Matches
            .Where(m => m.ClubId == clubId
                && m.Result != null
                && m.Players.Any(p => p.UserId == userId && p.Team == m.Result.WinnerTeam))
            .CountAsync();
    }

    public async Task<int> GetLossCountAsync(Guid clubId, Guid userId)
    {
        return await _db.Matches
            .Where(m => m.ClubId == clubId
                && m.Result != null
                && m.Result.WinnerTeam != null
                && m.Players.Any(p => p.UserId == userId && p.Team != m.Result.WinnerTeam))
            .CountAsync();
    }

    public async Task<int> GetTotalMatchCountAsync(Guid clubId, Guid userId)
    {
        return await _db.Matches
            .Where(m => m.ClubId == clubId && m.Players.Any(p => p.UserId == userId))
            .CountAsync();
    }
}
