using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs")]
public class ClubsController : ControllerBase
{
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberRepository _memberRepository;
    private readonly ICourtSettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUserService;

    public ClubsController(
        IClubRepository clubRepository,
        IClubMemberRepository memberRepository,
        ICourtSettingsRepository settingsRepository,
        ICurrentUserService currentUserService)
    {
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
        _settingsRepository = settingsRepository;
        _currentUserService = currentUserService;
    }

    /// <summary>Get all active clubs (public).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clubs = await _clubRepository.GetAllActiveAsync();
        return Ok(clubs.Select(MapToResponse));
    }

    /// <summary>Get a single club by ID (public).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        if (club is null)
            return NotFound(new { message = $"Club {id} not found." });

        return Ok(MapToResponse(club));
    }

    /// <summary>Create a new club. The creator automatically becomes the Admin member.</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClubRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var club = new Club
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            LogoUrl = request.LogoUrl
        };

        var created = await _clubRepository.CreateAsync(club);

        // Add creator as Admin member
        var member = new ClubMember
        {
            ClubId = created.Id,
            UserId = userId.Value,
            Role = Domain.Enums.ClubRole.Admin,
            Status = Domain.Enums.MemberStatus.Approved,
            JoinedAt = DateTime.UtcNow
        };
        await _memberRepository.CreateAsync(member);

        // Create default court settings
        var settings = new CourtSettings
        {
            ClubId = created.Id
        };
        await _settingsRepository.CreateAsync(settings);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
    }

    /// <summary>Update a club. Only club Admin.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClubRequest request)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        if (club is null)
            return NotFound(new { message = $"Club {id} not found." });

        if (request.Name is not null) club.Name = request.Name;
        if (request.Description is not null) club.Description = request.Description;
        if (request.Address is not null) club.Address = request.Address;
        if (request.City is not null) club.City = request.City;
        if (request.Country is not null) club.Country = request.Country;
        if (request.LogoUrl is not null) club.LogoUrl = request.LogoUrl;

        await _clubRepository.UpdateAsync(club);
        return Ok(MapToResponse(club));
    }

    /// <summary>Deactivate a club (soft delete). Only club Admin.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        if (club is null)
            return NotFound(new { message = $"Club {id} not found." });

        await _clubRepository.DeactivateAsync(id);
        return NoContent();
    }

    /// <summary>Get court settings for a club. Only club members.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}/settings")]
    public async Task<IActionResult> GetSettings(Guid id)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        if (club is null)
            return NotFound(new { message = $"Club {id} not found." });

        var settings = await _settingsRepository.GetByClubIdAsync(id);
        if (settings is null)
            return NotFound(new { message = "Court settings not found." });

        return Ok(MapSettingsToResponse(settings));
    }

    /// <summary>Update court settings for a club. Only club Admin.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateSettings(Guid id, [FromBody] UpdateCourtSettingsRequest request)
    {
        if (request.MinReservationMinutes > request.MaxReservationMinutes)
            return BadRequest(new { message = "MinReservationMinutes cannot exceed MaxReservationMinutes." });

        var club = await _clubRepository.GetByIdAsync(id);
        if (club is null)
            return NotFound(new { message = $"Club {id} not found." });

        var settings = await _settingsRepository.GetByClubIdAsync(id);
        if (settings is null)
        {
            settings = new CourtSettings
            {
                ClubId = id,
                MinReservationMinutes = request.MinReservationMinutes,
                MaxReservationMinutes = request.MaxReservationMinutes,
                CancellationDeadlineHours = request.CancellationDeadlineHours
            };
            await _settingsRepository.CreateAsync(settings);
        }
        else
        {
            settings.MinReservationMinutes = request.MinReservationMinutes;
            settings.MaxReservationMinutes = request.MaxReservationMinutes;
            settings.CancellationDeadlineHours = request.CancellationDeadlineHours;
            await _settingsRepository.UpdateAsync(settings);
        }

        return Ok(MapSettingsToResponse(settings));
    }

    private static ClubResponse MapToResponse(Club club) => new()
    {
        Id = club.Id,
        Name = club.Name,
        Description = club.Description,
        Address = club.Address,
        City = club.City,
        Country = club.Country,
        LogoUrl = club.LogoUrl,
        IsActive = club.IsActive,
        CreatedAt = club.CreatedAt,
        UpdatedAt = club.UpdatedAt
    };

    private static CourtSettingsResponse MapSettingsToResponse(CourtSettings s) => new()
    {
        Id = s.Id,
        ClubId = s.ClubId,
        MinReservationMinutes = s.MinReservationMinutes,
        MaxReservationMinutes = s.MaxReservationMinutes,
        CancellationDeadlineHours = s.CancellationDeadlineHours
    };
}
