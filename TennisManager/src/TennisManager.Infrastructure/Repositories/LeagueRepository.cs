using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly AppDbContext _db;

    public LeagueRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<League?> GetByIdAsync(Guid id)
    {
        return await _db.Leagues
            .Include(l => l.Club)
            .Include(l => l.CreatedByUser)
            .Include(l => l.Participants)
                .ThenInclude(p => p.User)
            .Include(l => l.LeagueMatches)
                .ThenInclude(lm => lm.Match)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<League>> GetByClubIdAsync(Guid clubId)
    {
        return await _db.Leagues
            .Include(l => l.Club)
            .Include(l => l.CreatedByUser)
            .Include(l => l.Participants)
            .Where(l => l.ClubId == clubId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<League> CreateAsync(League league)
    {
        league.Id = Guid.NewGuid();
        league.CreatedAt = DateTime.UtcNow;
        league.UpdatedAt = DateTime.UtcNow;
        _db.Leagues.Add(league);
        await _db.SaveChangesAsync();
        return league;
    }

    public async Task UpdateAsync(League league)
    {
        league.UpdatedAt = DateTime.UtcNow;
        _db.Leagues.Update(league);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var league = await _db.Leagues.FindAsync(id);
        if (league is not null)
        {
            _db.Leagues.Remove(league);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<LeagueParticipant>> GetParticipantsAsync(Guid leagueId)
    {
        return await _db.LeagueParticipants
            .Include(lp => lp.User)
            .Where(lp => lp.LeagueId == leagueId)
            .OrderBy(lp => lp.RegisteredAt)
            .ToListAsync();
    }

    public async Task AddParticipantAsync(LeagueParticipant participant)
    {
        participant.Id = Guid.NewGuid();
        participant.RegisteredAt = DateTime.UtcNow;
        _db.LeagueParticipants.Add(participant);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(Guid leagueId, Guid userId)
    {
        var participant = await _db.LeagueParticipants
            .FirstOrDefaultAsync(lp => lp.LeagueId == leagueId && lp.UserId == userId);
        if (participant is not null)
        {
            _db.LeagueParticipants.Remove(participant);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> IsParticipantAsync(Guid leagueId, Guid userId)
    {
        return await _db.LeagueParticipants
            .AnyAsync(lp => lp.LeagueId == leagueId && lp.UserId == userId);
    }

    public async Task<IEnumerable<LeagueMatch>> GetLeagueMatchesAsync(Guid leagueId)
    {
        return await _db.LeagueMatches
            .Include(lm => lm.Match)
                .ThenInclude(m => m.Players)
                    .ThenInclude(p => p.User)
            .Include(lm => lm.Match)
                .ThenInclude(m => m.Result)
            .Where(lm => lm.LeagueId == leagueId)
            .OrderBy(lm => lm.Round)
            .ThenBy(lm => lm.GroupName)
            .ToListAsync();
    }

    public async Task AddLeagueMatchAsync(LeagueMatch leagueMatch)
    {
        leagueMatch.Id = Guid.NewGuid();
        leagueMatch.CreatedAt = DateTime.UtcNow;
        _db.LeagueMatches.Add(leagueMatch);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<LeagueStanding>> GetStandingsAsync(Guid leagueId)
    {
        var participants = await _db.LeagueParticipants
            .Where(lp => lp.LeagueId == leagueId)
            .Select(lp => lp.UserId)
            .ToListAsync();

        var leagueMatchIds = await _db.LeagueMatches
            .Where(lm => lm.LeagueId == leagueId)
            .Select(lm => lm.MatchId)
            .ToListAsync();

        var matchResults = await _db.MatchResults
            .Where(mr => leagueMatchIds.Contains(mr.MatchId))
            .ToListAsync();

        var matchPlayers = await _db.MatchPlayers
            .Where(mp => leagueMatchIds.Contains(mp.MatchId))
            .ToListAsync();

        var standings = new List<LeagueStanding>();

        foreach (var userId in participants)
        {
            var playerMatchIds = matchPlayers
                .Where(mp => mp.UserId == userId)
                .Select(mp => mp.MatchId)
                .ToHashSet();

            int wins = 0, losses = 0, draws = 0;

            foreach (var matchId in playerMatchIds)
            {
                var result = matchResults.FirstOrDefault(r => r.MatchId == matchId);
                if (result is null)
                    continue;

                var playerTeam = matchPlayers
                    .FirstOrDefault(mp => mp.MatchId == matchId && mp.UserId == userId)?.Team;

                if (result.WinnerTeam is null)
                {
                    draws++;
                }
                else if (result.WinnerTeam == playerTeam)
                {
                    wins++;
                }
                else
                {
                    losses++;
                }
            }

            standings.Add(new LeagueStanding
            {
                UserId = userId,
                Wins = wins,
                Losses = losses,
                Draws = draws,
                Points = wins * 3 + draws * 1,
                MatchesPlayed = wins + losses + draws
            });
        }

        return standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.Wins);
    }
}
