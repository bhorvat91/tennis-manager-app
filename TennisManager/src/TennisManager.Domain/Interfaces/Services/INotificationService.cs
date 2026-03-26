using TennisManager.Domain.Entities;

namespace TennisManager.Domain.Interfaces.Services;

public interface INotificationService
{
    // Kreiranje notifikacija
    Task SendReservationConfirmationAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt, DateTime endsAt);
    Task SendReservationReminderAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt);
    Task SendReservationCancellationAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt);
    Task SendMatchResultNotificationAsync(Guid userId, Guid matchId, string resultText);
    Task SendLeagueMatchScheduledAsync(Guid userId, Guid leagueId, string leagueName, DateTime matchDate);
    Task SendMembershipApprovedAsync(Guid userId, Guid clubId, string clubName);
    Task SendMembershipRejectedAsync(Guid userId, Guid clubId, string clubName);

    // Dohvat notifikacija za korisnika
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
}
