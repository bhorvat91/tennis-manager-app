using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs/{clubId:guid}/leagues")]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUserService;

    public LeaguesController(
        ILeagueRepository leagueRepository,
        IClubRepository clubRepository,
        IClubMemberRepository memberRepository,
        ICurrentUserService currentUserService)
    {
        _leagueRepository = leagueRepository;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
        _currentUserService = currentUserService;
    }

    /// <summary>Get all leagues for a club.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clubId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var leagues = await _leagueRepository.GetByClubIdAsync(clubId);
        return Ok(leagues.Select(MapToResponse));
    }

    /// <summary>Get a single league by ID.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid clubId, Guid id)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        return Ok(MapToResponse(league));
    }

    /// <summary>Create a new league.</summary>
    [Authorize(Policy = "ClubCoachOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(Guid clubId, [FromBody] CreateLeagueRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "League name is required." });

        var league = new League
        {
            ClubId = clubId,
            Name = request.Name,
            Description = request.Description,
            Format = request.Format,
            MatchType = request.MatchType,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            CreatedBy = userId.Value
        };

        var created = await _leagueRepository.CreateAsync(league);
        var result = await _leagueRepository.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { clubId, id = created.Id }, MapToResponse(result!));
    }

    /// <summary>Update a league.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid clubId, Guid id, [FromBody] UpdateLeagueRequest request)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        if (request.Name is not null) league.Name = request.Name;
        if (request.Description is not null) league.Description = request.Description;
        if (request.Format.HasValue) league.Format = request.Format.Value;
        if (request.MatchType.HasValue) league.MatchType = request.MatchType.Value;
        if (request.Status.HasValue) league.Status = request.Status.Value;
        if (request.StartsAt.HasValue) league.StartsAt = request.StartsAt.Value;
        if (request.EndsAt.HasValue) league.EndsAt = request.EndsAt.Value;

        await _leagueRepository.UpdateAsync(league);
        var updated = await _leagueRepository.GetByIdAsync(id);
        return Ok(MapToResponse(updated!));
    }

    /// <summary>Cancel (soft-delete) a league by setting status to Cancelled.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid clubId, Guid id)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        league.Status = LeagueStatus.Cancelled;
        await _leagueRepository.UpdateAsync(league);
        return NoContent();
    }

    /// <summary>Get participants of a league.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}/participants")]
    public async Task<IActionResult> GetParticipants(Guid clubId, Guid id)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        var participants = await _leagueRepository.GetParticipantsAsync(id);
        return Ok(participants.Select(p => new LeagueParticipantResponse
        {
            UserId = p.UserId,
            FirstName = p.User?.FirstName ?? string.Empty,
            LastName = p.User?.LastName ?? string.Empty,
            Email = p.User?.Email ?? string.Empty,
            RegisteredAt = p.RegisteredAt
        }));
    }

    /// <summary>Register current user as a participant in a league.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPost("{id:guid}/participants")]
    public async Task<IActionResult> AddParticipant(Guid clubId, Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        if (league.Status != LeagueStatus.Draft && league.Status != LeagueStatus.Active)
            return BadRequest(new { message = "League is not accepting participants." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        var isAlready = await _leagueRepository.IsParticipantAsync(id, userId.Value);
        if (isAlready)
            return Conflict(new { message = "User is already registered in this league." });

        await _leagueRepository.AddParticipantAsync(new LeagueParticipant
        {
            LeagueId = id,
            UserId = userId.Value
        });

        return Ok(new { message = "Successfully registered in the league." });
    }

    /// <summary>Remove a participant from a league. Only the participant themselves or an Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpDelete("{id:guid}/participants/{userId:guid}")]
    public async Task<IActionResult> RemoveParticipant(Guid clubId, Guid id, Guid userId)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId is null)
            return Unauthorized();

        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, currentUserId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (currentUserId.Value != userId && member.Role != ClubRole.Admin)
            return Forbid();

        var isParticipant = await _leagueRepository.IsParticipantAsync(id, userId);
        if (!isParticipant)
            return NotFound(new { message = "Participant not found in this league." });

        await _leagueRepository.RemoveParticipantAsync(id, userId);
        return NoContent();
    }

    /// <summary>Get matches linked to a league.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}/matches")]
    public async Task<IActionResult> GetMatches(Guid clubId, Guid id)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        var leagueMatches = await _leagueRepository.GetLeagueMatchesAsync(id);
        return Ok(leagueMatches.Select(lm => new LeagueMatchResponse
        {
            Id = lm.Id,
            LeagueId = lm.LeagueId,
            MatchId = lm.MatchId,
            Round = lm.Round,
            GroupName = lm.GroupName,
            IsRequired = lm.IsRequired,
            Match = lm.Match is not null ? new MatchSummaryResponse
            {
                Id = lm.Match.Id,
                MatchType = lm.Match.MatchType,
                PlayedAt = lm.Match.PlayedAt,
                Notes = lm.Match.Notes,
                WinnerTeam = lm.Match.Result?.WinnerTeam,
                Players = lm.Match.Players.Select(p => new MatchPlayerResponse
                {
                    UserId = p.UserId,
                    FirstName = p.User?.FirstName ?? string.Empty,
                    LastName = p.User?.LastName ?? string.Empty,
                    Team = p.Team
                }).ToList()
            } : null
        }));
    }

    /// <summary>Get standings for a league.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}/standings")]
    public async Task<IActionResult> GetStandings(Guid clubId, Guid id)
    {
        var league = await _leagueRepository.GetByIdAsync(id);
        if (league is null || league.ClubId != clubId)
            return NotFound(new { message = $"League {id} not found." });

        var participants = await _leagueRepository.GetParticipantsAsync(id);
        var participantMap = participants.ToDictionary(p => p.UserId, p => p.User);

        var standings = await _leagueRepository.GetStandingsAsync(id);

        var result = standings
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.Wins)
            .Select((s, index) =>
            {
                participantMap.TryGetValue(s.UserId, out var user);
                return new StandingResponse
                {
                    Position = index + 1,
                    UserId = s.UserId,
                    FirstName = user?.FirstName ?? string.Empty,
                    LastName = user?.LastName ?? string.Empty,
                    Wins = s.Wins,
                    Losses = s.Losses,
                    Draws = s.Draws,
                    Points = s.Points,
                    MatchesPlayed = s.MatchesPlayed
                };
            })
            .ToList();

        return Ok(result);
    }

    private static LeagueResponse MapToResponse(League l) => new()
    {
        Id = l.Id,
        ClubId = l.ClubId,
        Name = l.Name,
        Description = l.Description,
        Format = l.Format,
        MatchType = l.MatchType,
        Status = l.Status,
        StartsAt = l.StartsAt,
        EndsAt = l.EndsAt,
        CreatedBy = l.CreatedBy,
        CreatedAt = l.CreatedAt,
        ParticipantCount = l.Participants.Count
    };
}
