using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class UserService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<UserProfileDto>("users/profile");
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var client = CreateClient();
        var response = await client.PutAsJsonAsync("users/profile", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserProfileDto>();
    }
}
