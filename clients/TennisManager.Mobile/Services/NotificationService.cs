using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class NotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<NotificationDto>> GetNotificationsAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<NotificationDto>>("notifications");
        return result ?? new List<NotificationDto>();
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<UnreadCountDto>("notifications/unread-count");
        return result?.Count ?? 0;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var client = CreateClient();
        var response = await client.PutAsync($"notifications/{notificationId}/read", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkAllAsReadAsync()
    {
        var client = CreateClient();
        var response = await client.PutAsync("notifications/read-all", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"notifications/{notificationId}");
        response.EnsureSuccessStatusCode();
    }

    // TODO: Push notifications via Firebase FCM
    // public async Task RegisterDeviceTokenAsync(string deviceToken) { }
}
