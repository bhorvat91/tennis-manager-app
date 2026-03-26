using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Reservation>> GetByClubIdAsync(Guid clubId);
    Task<IEnumerable<Reservation>> GetByCourtIdAsync(Guid courtId);
    Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid clubId, Guid userId);
    Task<IEnumerable<Reservation>> GetByCourtAndDateAsync(Guid courtId, DateTime date);
    Task<bool> HasOverlappingReservationAsync(Guid courtId, DateTime startsAt, DateTime endsAt, Guid? excludeReservationId = null);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task AddParticipantAsync(ReservationParticipant participant);
    Task RemoveParticipantAsync(Guid reservationId, Guid userId);
    Task<IEnumerable<ReservationParticipant>> GetParticipantsAsync(Guid reservationId);
}
