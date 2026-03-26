using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TennisManager.Mobile.Models;
using TennisManager.Mobile.Services;

namespace TennisManager.Mobile.ViewModels.Notifications;

public partial class NotificationListViewModel : ObservableObject
{
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<NotificationDto> _notifications = new();

    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public NotificationListViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task LoadNotificationsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var notifications = await _notificationService.GetNotificationsAsync();
            Notifications = new ObservableCollection<NotificationDto>(notifications.OrderByDescending(n => n.CreatedAt));
            UnreadCount = notifications.Count(n => !n.IsRead);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće učitati notifikacije.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task MarkAsReadAsync(NotificationDto notification)
    {
        if (notification.IsRead) return;
        try
        {
            await _notificationService.MarkAsReadAsync(notification.Id);
            notification.IsRead = true;
            UnreadCount = Math.Max(0, UnreadCount - 1);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće označiti notifikaciju kao pročitanu.";
        }
    }

    [RelayCommand]
    private async Task MarkAllAsReadAsync()
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync();
            foreach (var n in Notifications)
                n.IsRead = true;
            UnreadCount = 0;
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće označiti sve notifikacije kao pročitane.";
        }
    }

    [RelayCommand]
    private async Task DeleteNotificationAsync(NotificationDto notification)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(notification.Id);
            Notifications.Remove(notification);
            if (!notification.IsRead)
                UnreadCount = Math.Max(0, UnreadCount - 1);
        }
        catch (Exception)
        {
            ErrorMessage = "Nije moguće obrisati notifikaciju.";
        }
    }
}
