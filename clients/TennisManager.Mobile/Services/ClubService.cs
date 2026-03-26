using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class ClubService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ClubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<ClubDto>> GetClubsAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<ClubDto>>("clubs");
        return result ?? new List<ClubDto>();
    }

    public async Task<ClubDto?> GetClubAsync(Guid clubId)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<ClubDto>($"clubs/{clubId}");
    }

    public async Task<List<ClubDto>> GetMyClubsAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<ClubDto>>("clubs/my");
        return result ?? new List<ClubDto>();
    }

    public async Task RequestMembershipAsync(Guid clubId, RequestMembershipRequest request)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"clubs/{clubId}/members/request", request);
        response.EnsureSuccessStatusCode();
    }
}
