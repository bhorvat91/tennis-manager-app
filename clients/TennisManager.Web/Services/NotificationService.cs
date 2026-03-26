using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class NotificationService
{
    private readonly HttpClient _httpClient;

    public NotificationService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<NotificationDto>> GetNotificationsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<NotificationDto>>("notifications");
        return result ?? new List<NotificationDto>();
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<UnreadCountDto>("notifications/unread-count");
        return result?.Count ?? 0;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var response = await _httpClient.PostAsync($"notifications/{notificationId}/read", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkAllAsReadAsync()
    {
        var response = await _httpClient.PostAsync("notifications/read-all", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        var response = await _httpClient.DeleteAsync($"notifications/{notificationId}");
        response.EnsureSuccessStatusCode();
    }
}
