using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Domain.Interfaces.Services;
using MatchType = TennisManager.Domain.Enums.MatchType;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs/{clubId:guid}/matches")]
public class MatchesController : ControllerBase
{
    private readonly IMatchRepository _matchRepository;
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberRepository _memberRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public MatchesController(
        IMatchRepository matchRepository,
        IClubRepository clubRepository,
        IClubMemberRepository memberRepository,
        IReservationRepository reservationRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _matchRepository = matchRepository;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    /// <summary>Get all matches for a club.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clubId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var matches = await _matchRepository.GetByClubIdAsync(clubId);
        return Ok(matches.Select(MapToResponse));
    }

    /// <summary>Get my matches in this club.</summary>
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy(Guid clubId)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var matches = await _matchRepository.GetByUserIdAsync(clubId, userId.Value);
        return Ok(matches.Select(MapToResponse));
    }

    /// <summary>Get a single match by ID.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid clubId, Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null || match.ClubId != clubId)
            return NotFound(new { message = $"Match {id} not found." });

        return Ok(MapToResponse(match));
    }

    /// <summary>Get player statistics (wins/losses) in this club.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("stats/{userId:guid}")]
    public async Task<IActionResult> GetStats(Guid clubId, Guid userId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var total = await _matchRepository.GetTotalMatchCountAsync(clubId, userId);
        var wins = await _matchRepository.GetWinCountAsync(clubId, userId);
        var losses = await _matchRepository.GetLossCountAsync(clubId, userId);

        return Ok(new PlayerStatsResponse
        {
            UserId = userId,
            TotalMatches = total,
            Wins = wins,
            Losses = losses,
            WinPercentage = total > 0 ? Math.Round((double)wins / total * 100, 2) : 0
        });
    }

    /// <summary>Create a new match.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPost]
    public async Task<IActionResult> Create(Guid clubId, [FromBody] CreateMatchRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        // Validate reservation if provided
        if (request.ReservationId.HasValue)
        {
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId.Value);
            if (reservation is null || reservation.ClubId != clubId)
                return BadRequest(new { message = "Reservation not found or does not belong to this club." });
        }

        // Validate team composition
        var validationError = ValidatePlayers(request.Players, request.MatchType);
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        // Validate all players are approved club members
        foreach (var playerReq in request.Players)
        {
            var playerMember = await _memberRepository.GetByClubAndUserAsync(clubId, playerReq.UserId);
            if (playerMember is null || playerMember.Status != MemberStatus.Approved)
                return BadRequest(new { message = $"Player {playerReq.UserId} is not an approved member of this club." });
        }

        var match = new Match
        {
            ClubId = clubId,
            ReservationId = request.ReservationId,
            MatchType = request.MatchType,
            PlayedAt = request.PlayedAt,
            Notes = request.Notes,
            CreatedBy = userId.Value
        };

        var created = await _matchRepository.CreateAsync(match);

        foreach (var playerReq in request.Players)
        {
            await _matchRepository.AddPlayerAsync(new MatchPlayer
            {
                MatchId = created.Id,
                UserId = playerReq.UserId,
                Team = playerReq.Team
            });
        }

        var result = await _matchRepository.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { clubId, id = created.Id }, MapToResponse(result!));
    }

    /// <summary>Update a match. Only creator or Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid clubId, Guid id, [FromBody] UpdateMatchRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null || match.ClubId != clubId)
            return NotFound(new { message = $"Match {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (match.CreatedBy != userId.Value && member.Role != ClubRole.Admin)
            return Forbid();

        if (request.MatchType.HasValue) match.MatchType = request.MatchType.Value;
        if (request.PlayedAt.HasValue) match.PlayedAt = request.PlayedAt.Value;
        if (request.Notes is not null) match.Notes = request.Notes;

        await _matchRepository.UpdateAsync(match);
        var updated = await _matchRepository.GetByIdAsync(id);
        return Ok(MapToResponse(updated!));
    }

    /// <summary>Delete a match. Only creator or Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid clubId, Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null || match.ClubId != clubId)
            return NotFound(new { message = $"Match {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (match.CreatedBy != userId.Value && member.Role != ClubRole.Admin)
            return Forbid();

        await _matchRepository.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Enter match result. Only approved club members, once per match.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPost("{id:guid}/result")]
    public async Task<IActionResult> CreateResult(Guid clubId, Guid id, [FromBody] CreateMatchResultRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null || match.ClubId != clubId)
            return NotFound(new { message = $"Match {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (match.Result is not null)
            return Conflict(new { message = "Result already exists. Use PUT to update." });

        if (request.WinnerTeam.HasValue && request.WinnerTeam != 1 && request.WinnerTeam != 2)
            return BadRequest(new { message = "WinnerTeam must be 1 or 2." });

        var result = new MatchResult
        {
            MatchId = id,
            WinnerTeam = request.WinnerTeam,
            ResultText = request.ResultText,
            EnteredBy = userId.Value
        };

        await _matchRepository.AddResultAsync(result);

        if (request.Sets is not null)
        {
            foreach (var setReq in request.Sets)
            {
                await _matchRepository.AddSetAsync(new MatchSet
                {
                    MatchId = id,
                    SetNumber = setReq.SetNumber,
                    Team1Games = setReq.Team1Games,
                    Team2Games = setReq.Team2Games
                });
            }
        }

        var updated = await _matchRepository.GetByIdAsync(id);

        // Send match result notifications to all players
        var resultSummary = request.ResultText ?? (request.WinnerTeam.HasValue ? $"Team {request.WinnerTeam} won" : "Result entered");
        foreach (var player in updated!.Players)
        {
            await _notificationService.SendMatchResultNotificationAsync(player.UserId, id, resultSummary);
        }

        return Ok(MapToResponse(updated!));
    }

    /// <summary>Update match result. Only the one who entered it or Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPut("{id:guid}/result")]
    public async Task<IActionResult> UpdateResult(Guid clubId, Guid id, [FromBody] UpdateMatchResultRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null || match.ClubId != clubId)
            return NotFound(new { message = $"Match {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        var existingResult = await _matchRepository.GetResultAsync(id);
        if (existingResult is null)
            return NotFound(new { message = "No result found for this match. Use POST to create one." });

        if (existingResult.EnteredBy != userId.Value && member.Role != ClubRole.Admin)
            return Forbid();

        if (request.WinnerTeam.HasValue && request.WinnerTeam != 1 && request.WinnerTeam != 2)
            return BadRequest(new { message = "WinnerTeam must be 1 or 2." });

        existingResult.WinnerTeam = request.WinnerTeam;
        existingResult.ResultText = request.ResultText;
        await _matchRepository.UpdateResultAsync(existingResult);

        if (request.Sets is not null)
        {
            await _matchRepository.DeleteSetsAsync(id);
            foreach (var setReq in request.Sets)
            {
                await _matchRepository.AddSetAsync(new MatchSet
                {
                    MatchId = id,
                    SetNumber = setReq.SetNumber,
                    Team1Games = setReq.Team1Games,
                    Team2Games = setReq.Team2Games
                });
            }
        }

        var updated = await _matchRepository.GetByIdAsync(id);
        return Ok(MapToResponse(updated!));
    }

    private static string? ValidatePlayers(List<MatchPlayerRequest> players, MatchType matchType)
    {
        if (players.Count == 0)
            return "At least one player must be added.";

        var team1 = players.Count(p => p.Team == 1);
        var team2 = players.Count(p => p.Team == 2);

        if (matchType == MatchType.Singles)
        {
            if (team1 != 1 || team2 != 1)
                return "Singles match requires exactly 1 player per team (2 total).";
        }
        else if (matchType == MatchType.Doubles)
        {
            if (team1 != 2 || team2 != 2)
                return "Doubles match requires exactly 2 players per team (4 total).";
        }

        var invalidTeams = players.Where(p => p.Team != 1 && p.Team != 2).ToList();
        if (invalidTeams.Count > 0)
            return "Team must be 1 or 2.";

        return null;
    }

    private static MatchResponse MapToResponse(Match m) => new()
    {
        Id = m.Id,
        ClubId = m.ClubId,
        ReservationId = m.ReservationId,
        MatchType = m.MatchType,
        PlayedAt = m.PlayedAt,
        Notes = m.Notes,
        CreatedBy = m.CreatedBy,
        CreatorName = m.Creator is not null ? $"{m.Creator.FirstName} {m.Creator.LastName}" : string.Empty,
        CreatedAt = m.CreatedAt,
        Players = m.Players.Select(p => new MatchPlayerResponse
        {
            UserId = p.UserId,
            FirstName = p.User?.FirstName ?? string.Empty,
            LastName = p.User?.LastName ?? string.Empty,
            Team = p.Team
        }).ToList(),
        Result = m.Result is not null ? new MatchResultResponse
        {
            Id = m.Result.Id,
            WinnerTeam = m.Result.WinnerTeam,
            ResultText = m.Result.ResultText,
            EnteredBy = m.Result.EnteredBy,
            EnteredAt = m.Result.EnteredAt,
            Sets = m.Sets.OrderBy(s => s.SetNumber).Select(s => new MatchSetResponse
            {
                SetNumber = s.SetNumber,
                Team1Games = s.Team1Games,
                Team2Games = s.Team2Games
            }).ToList()
        } : null
    };
}
