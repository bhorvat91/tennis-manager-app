using System.Text.Json;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Domain.Interfaces.Services;

namespace TennisManager.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task SendReservationConfirmationAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt, DateTime endsAt)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.ReservationConfirmed,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Reservation Confirmed",
            Body = $"Your reservation on court \"{courtName}\" on {startsAt:dd.MM.yyyy} from {startsAt:HH:mm} to {endsAt:HH:mm} has been confirmed.",
            Data = JsonSerializer.Serialize(new { reservationId, courtName, startsAt, endsAt })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendReservationReminderAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.ReservationReminder,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Reservation Reminder",
            Body = $"Reminder: You have a reservation on court \"{courtName}\" today at {startsAt:HH:mm}.",
            Data = JsonSerializer.Serialize(new { reservationId, courtName, startsAt })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendReservationCancellationAsync(Guid userId, Guid reservationId, string courtName, DateTime startsAt)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.ReservationCancelled,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Reservation Cancelled",
            Body = $"Your reservation on court \"{courtName}\" on {startsAt:dd.MM.yyyy} at {startsAt:HH:mm} has been cancelled.",
            Data = JsonSerializer.Serialize(new { reservationId, courtName, startsAt })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendMatchResultNotificationAsync(Guid userId, Guid matchId, string resultText)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.MatchResultEntered,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Match Result Entered",
            Body = $"The result for your match has been entered: {resultText}",
            Data = JsonSerializer.Serialize(new { matchId, resultText })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendLeagueMatchScheduledAsync(Guid userId, Guid leagueId, string leagueName, DateTime matchDate)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.LeagueMatchScheduled,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "League Match Scheduled",
            Body = $"A new match has been scheduled in league \"{leagueName}\" on {matchDate:dd.MM.yyyy} at {matchDate:HH:mm}.",
            Data = JsonSerializer.Serialize(new { leagueId, leagueName, matchDate })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendMembershipApprovedAsync(Guid userId, Guid clubId, string clubName)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.MembershipApproved,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Membership Approved",
            Body = $"Your membership request for club \"{clubName}\" has been approved.",
            Data = JsonSerializer.Serialize(new { clubId, clubName })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task SendMembershipRejectedAsync(Guid userId, Guid clubId, string clubName)
    {
        // TODO: Integrate email service (SendGrid/Resend)
        // TODO: Integrate push notifications (Firebase FCM)
        var notification = new Notification
        {
            UserId = userId,
            Type = NotificationType.MembershipRejected,
            Channel = NotificationChannel.InApp,
            Status = NotificationStatus.Pending,
            Title = "Membership Rejected",
            Body = $"Your membership request for club \"{clubName}\" has been rejected.",
            Data = JsonSerializer.Serialize(new { clubId, clubName })
        };
        await _notificationRepository.CreateAsync(notification);
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await _notificationRepository.GetByUserIdAsync(userId, page, pageSize);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification is not null && notification.UserId == userId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification is not null && notification.UserId == userId)
        {
            await _notificationRepository.DeleteAsync(notificationId);
        }
    }
}
