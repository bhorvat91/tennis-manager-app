using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Domain.Interfaces.Services;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs/{clubId:guid}/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberRepository _memberRepository;
    private readonly ICourtSettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public ReservationsController(
        IReservationRepository reservationRepository,
        ICourtRepository courtRepository,
        IClubRepository clubRepository,
        IClubMemberRepository memberRepository,
        ICourtSettingsRepository settingsRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _reservationRepository = reservationRepository;
        _courtRepository = courtRepository;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
        _settingsRepository = settingsRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    /// <summary>Get all reservations for a club. Optionally filter by courtId and date.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clubId, [FromQuery] Guid? courtId, [FromQuery] DateTime? date)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        IEnumerable<Reservation> reservations;

        if (courtId.HasValue && date.HasValue)
            reservations = await _reservationRepository.GetByCourtAndDateAsync(courtId.Value, date.Value);
        else if (courtId.HasValue)
            reservations = await _reservationRepository.GetByCourtIdAsync(courtId.Value);
        else
            reservations = await _reservationRepository.GetByClubIdAsync(clubId);

        return Ok(reservations.Select(MapToResponse));
    }

    /// <summary>Get my reservations in this club.</summary>
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

        var reservations = await _reservationRepository.GetByUserIdAsync(clubId, userId.Value);
        return Ok(reservations.Select(MapToResponse));
    }

    /// <summary>Get a single reservation by ID.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid clubId, Guid id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null || reservation.ClubId != clubId)
            return NotFound(new { message = $"Reservation {id} not found." });

        return Ok(MapToResponse(reservation));
    }

    /// <summary>Create a new reservation.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPost]
    public async Task<IActionResult> Create(Guid clubId, [FromBody] CreateReservationRequest request)
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

        var court = await _courtRepository.GetByIdAsync(request.CourtId);
        if (court is null || !court.IsActive || court.ClubId != clubId)
            return BadRequest(new { message = "Court not found or not active in this club." });

        var settings = await _settingsRepository.GetByClubIdAsync(clubId);
        if (settings is not null)
        {
            var durationMinutes = (int)(request.EndsAt - request.StartsAt).TotalMinutes;
            if (durationMinutes < settings.MinReservationMinutes)
                return BadRequest(new { message = $"Reservation duration must be at least {settings.MinReservationMinutes} minutes." });
            if (durationMinutes > settings.MaxReservationMinutes)
                return BadRequest(new { message = $"Reservation duration cannot exceed {settings.MaxReservationMinutes} minutes." });
        }

        if (request.EndsAt <= request.StartsAt)
            return BadRequest(new { message = "End time must be after start time." });

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(request.CourtId, request.StartsAt, request.EndsAt);
        if (hasOverlap)
            return Conflict(new { message = "The court is already reserved for this time slot." });

        var reservation = new Reservation
        {
            ClubId = clubId,
            CourtId = request.CourtId,
            CreatedBy = userId.Value,
            ReservationType = request.ReservationType,
            Status = ReservationStatus.Confirmed,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            Notes = request.Notes
        };

        var created = await _reservationRepository.CreateAsync(reservation);

        if (request.ParticipantUserIds is not null)
        {
            foreach (var participantId in request.ParticipantUserIds.Distinct())
            {
                await _reservationRepository.AddParticipantAsync(new ReservationParticipant
                {
                    ReservationId = created.Id,
                    UserId = participantId
                });
            }
        }

        var result = await _reservationRepository.GetByIdAsync(created.Id);

        // Send confirmation notifications to all participants
        var courtName = court.Name;
        await _notificationService.SendReservationConfirmationAsync(userId.Value, created.Id, courtName, created.StartsAt, created.EndsAt);
        if (request.ParticipantUserIds is not null)
        {
            foreach (var participantId in request.ParticipantUserIds.Distinct().Where(id => id != userId.Value))
            {
                await _notificationService.SendReservationConfirmationAsync(participantId, created.Id, courtName, created.StartsAt, created.EndsAt);
            }
        }

        return CreatedAtAction(nameof(GetById), new { clubId, id = created.Id }, MapToResponse(result!));
    }

    /// <summary>Update a reservation. Only creator or Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid clubId, Guid id, [FromBody] UpdateReservationRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null || reservation.ClubId != clubId)
            return NotFound(new { message = $"Reservation {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (reservation.CreatedBy != userId.Value && member.Role != ClubRole.Admin)
            return Forbid();

        if (reservation.Status == ReservationStatus.Cancelled)
            return BadRequest(new { message = "Cannot update a cancelled reservation." });

        var newStartsAt = request.StartsAt ?? reservation.StartsAt;
        var newEndsAt = request.EndsAt ?? reservation.EndsAt;

        if (newEndsAt <= newStartsAt)
            return BadRequest(new { message = "End time must be after start time." });

        var settings = await _settingsRepository.GetByClubIdAsync(clubId);
        if (settings is not null)
        {
            var durationMinutes = (int)(newEndsAt - newStartsAt).TotalMinutes;
            if (durationMinutes < settings.MinReservationMinutes)
                return BadRequest(new { message = $"Reservation duration must be at least {settings.MinReservationMinutes} minutes." });
            if (durationMinutes > settings.MaxReservationMinutes)
                return BadRequest(new { message = $"Reservation duration cannot exceed {settings.MaxReservationMinutes} minutes." });
        }

        if (request.StartsAt.HasValue || request.EndsAt.HasValue)
        {
            var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(reservation.CourtId, newStartsAt, newEndsAt, id);
            if (hasOverlap)
                return Conflict(new { message = "The court is already reserved for this time slot." });
        }

        if (request.StartsAt.HasValue) reservation.StartsAt = request.StartsAt.Value;
        if (request.EndsAt.HasValue) reservation.EndsAt = request.EndsAt.Value;
        if (request.Notes is not null) reservation.Notes = request.Notes;
        if (request.ReservationType.HasValue) reservation.ReservationType = request.ReservationType.Value;

        await _reservationRepository.UpdateAsync(reservation);
        return Ok(MapToResponse(reservation));
    }

    /// <summary>Cancel a reservation. Only creator or Admin.</summary>
    [Authorize(Policy = "ClubMember")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid clubId, Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null || reservation.ClubId != clubId)
            return NotFound(new { message = $"Reservation {id} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null || member.Status != MemberStatus.Approved)
            return Forbid();

        if (reservation.CreatedBy != userId.Value && member.Role != ClubRole.Admin)
            return Forbid();

        if (reservation.Status == ReservationStatus.Cancelled)
            return BadRequest(new { message = "Reservation is already cancelled." });

        var settings = await _settingsRepository.GetByClubIdAsync(clubId);
        if (settings is not null && member.Role != ClubRole.Admin)
        {
            var hoursUntilStart = (reservation.StartsAt - DateTime.UtcNow).TotalHours;
            if (hoursUntilStart < settings.CancellationDeadlineHours)
                return BadRequest(new { message = $"Reservations can only be cancelled at least {settings.CancellationDeadlineHours} hours before the start time." });
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.CancelledBy = userId.Value;
        await _reservationRepository.UpdateAsync(reservation);

        // Send cancellation notifications to creator and all participants
        var courtName = reservation.Court?.Name ?? string.Empty;
        await _notificationService.SendReservationCancellationAsync(reservation.CreatedBy, reservation.Id, courtName, reservation.StartsAt);
        foreach (var participant in reservation.Participants.Where(p => p.UserId != reservation.CreatedBy))
        {
            await _notificationService.SendReservationCancellationAsync(participant.UserId, reservation.Id, courtName, reservation.StartsAt);
        }

        return NoContent();
    }

    /// <summary>Get available time slots for a court on a given date.</summary>
    [Authorize(Policy = "ClubMember")]
    [Route("/api/v1/clubs/{clubId:guid}/courts/{courtId:guid}/availability")]
    [HttpGet]
    public async Task<IActionResult> GetAvailability(Guid clubId, Guid courtId, [FromQuery] DateTime date)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var court = await _courtRepository.GetByIdAsync(courtId);
        if (court is null || !court.IsActive || court.ClubId != clubId)
            return NotFound(new { message = $"Court {courtId} not found." });

        var settings = await _settingsRepository.GetByClubIdAsync(clubId);
        var slotMinutes = settings?.MinReservationMinutes ?? 60;

        var existingReservations = await _reservationRepository.GetByCourtAndDateAsync(courtId, date.Date);

        var dayStart = date.Date.AddHours(7);
        var dayEnd = date.Date.AddHours(22);
        var slots = new List<AvailabilitySlotResponse>();

        var current = dayStart;
        while (current.AddMinutes(slotMinutes) <= dayEnd)
        {
            var slotEnd = current.AddMinutes(slotMinutes);
            var isOccupied = existingReservations.Any(r =>
                r.StartsAt < slotEnd && r.EndsAt > current);

            if (!isOccupied)
            {
                slots.Add(new AvailabilitySlotResponse
                {
                    StartsAt = current,
                    EndsAt = slotEnd,
                    DurationMinutes = slotMinutes
                });
            }

            current = slotEnd;
        }

        return Ok(slots);
    }

    private static ReservationResponse MapToResponse(Reservation r) => new()
    {
        Id = r.Id,
        ClubId = r.ClubId,
        CourtId = r.CourtId,
        CourtName = r.Court?.Name ?? string.Empty,
        CreatedBy = r.CreatedBy,
        CreatorName = r.Creator is not null ? $"{r.Creator.FirstName} {r.Creator.LastName}" : string.Empty,
        ReservationType = r.ReservationType,
        Status = r.Status,
        StartsAt = r.StartsAt,
        EndsAt = r.EndsAt,
        Notes = r.Notes,
        CreatedAt = r.CreatedAt,
        Participants = r.Participants.Select(p => new ParticipantResponse
        {
            UserId = p.UserId,
            FirstName = p.User?.FirstName ?? string.Empty,
            LastName = p.User?.LastName ?? string.Empty,
            Email = p.User?.Email ?? string.Empty
        }).ToList()
    };
}
