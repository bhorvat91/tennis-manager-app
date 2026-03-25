using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _db;

    public ReservationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _db.Reservations
            .Include(r => r.Court)
            .Include(r => r.Creator)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetByClubIdAsync(Guid clubId)
    {
        return await _db.Reservations
            .Include(r => r.Court)
            .Include(r => r.Creator)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.ClubId == clubId)
            .OrderBy(r => r.StartsAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByCourtIdAsync(Guid courtId)
    {
        return await _db.Reservations
            .Include(r => r.Court)
            .Include(r => r.Creator)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.CourtId == courtId)
            .OrderBy(r => r.StartsAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid clubId, Guid userId)
    {
        return await _db.Reservations
            .Include(r => r.Court)
            .Include(r => r.Creator)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.ClubId == clubId && r.CreatedBy == userId)
            .OrderBy(r => r.StartsAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByCourtAndDateAsync(Guid courtId, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _db.Reservations
            .Include(r => r.Court)
            .Include(r => r.Creator)
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.CourtId == courtId
                && r.Status != ReservationStatus.Cancelled
                && r.StartsAt >= dayStart
                && r.StartsAt < dayEnd)
            .OrderBy(r => r.StartsAt)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingReservationAsync(Guid courtId, DateTime startsAt, DateTime endsAt, Guid? excludeReservationId = null)
    {
        var query = _db.Reservations
            .Where(r => r.CourtId == courtId
                && r.Status == ReservationStatus.Confirmed
                && r.StartsAt < endsAt
                && r.EndsAt > startsAt);

        if (excludeReservationId.HasValue)
            query = query.Where(r => r.Id != excludeReservationId.Value);

        return await query.AnyAsync();
    }

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        reservation.Id = Guid.NewGuid();
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return reservation;
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        reservation.UpdatedAt = DateTime.UtcNow;
        _db.Reservations.Update(reservation);
        await _db.SaveChangesAsync();
    }

    public async Task AddParticipantAsync(ReservationParticipant participant)
    {
        _db.ReservationParticipants.Add(participant);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(Guid reservationId, Guid userId)
    {
        var participant = await _db.ReservationParticipants
            .FirstOrDefaultAsync(p => p.ReservationId == reservationId && p.UserId == userId);
        if (participant is not null)
        {
            _db.ReservationParticipants.Remove(participant);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ReservationParticipant>> GetParticipantsAsync(Guid reservationId)
    {
        return await _db.ReservationParticipants
            .Include(p => p.User)
            .Where(p => p.ReservationId == reservationId)
            .ToListAsync();
    }
}
