using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs/{clubId:guid}/courts")]
public class CourtsController : ControllerBase
{
    private readonly ICourtRepository _courtRepository;
    private readonly IClubRepository _clubRepository;

    public CourtsController(ICourtRepository courtRepository, IClubRepository clubRepository)
    {
        _courtRepository = courtRepository;
        _clubRepository = clubRepository;
    }

    /// <summary>Get all active courts for a club. Club members only.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clubId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var courts = await _courtRepository.GetByClubIdAsync(clubId);
        return Ok(courts.Select(MapToResponse));
    }

    /// <summary>Get a single court by ID. Club members only.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid clubId, Guid id)
    {
        var court = await _courtRepository.GetByIdAsync(id);
        if (court is null || court.ClubId != clubId)
            return NotFound(new { message = $"Court {id} not found." });

        return Ok(MapToResponse(court));
    }

    /// <summary>Create a new court. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(Guid clubId, [FromBody] CreateCourtRequest request)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var court = new Court
        {
            ClubId = clubId,
            Name = request.Name,
            Surface = request.Surface,
            Environment = request.Environment
        };

        var created = await _courtRepository.CreateAsync(court);
        return CreatedAtAction(nameof(GetById), new { clubId, id = created.Id }, MapToResponse(created));
    }

    /// <summary>Update a court. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid clubId, Guid id, [FromBody] UpdateCourtRequest request)
    {
        var court = await _courtRepository.GetByIdAsync(id);
        if (court is null || court.ClubId != clubId)
            return NotFound(new { message = $"Court {id} not found." });

        if (request.Name is not null) court.Name = request.Name;
        if (request.Surface.HasValue) court.Surface = request.Surface.Value;
        if (request.Environment.HasValue) court.Environment = request.Environment.Value;

        await _courtRepository.UpdateAsync(court);
        return Ok(MapToResponse(court));
    }

    /// <summary>Deactivate a court (soft delete). Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid clubId, Guid id)
    {
        var court = await _courtRepository.GetByIdAsync(id);
        if (court is null || court.ClubId != clubId)
            return NotFound(new { message = $"Court {id} not found." });

        await _courtRepository.DeactivateAsync(id);
        return NoContent();
    }

    private static CourtResponse MapToResponse(Court c) => new()
    {
        Id = c.Id,
        ClubId = c.ClubId,
        Name = c.Name,
        Surface = c.Surface,
        Environment = c.Environment,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
