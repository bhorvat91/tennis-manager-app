using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<UserProfile?> GetCurrentUserAsync()
        => await _httpClient.GetFromJsonAsync<UserProfile>("users/me");

    public async Task<UserProfile?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("users/me", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserProfile>();
    }
}
