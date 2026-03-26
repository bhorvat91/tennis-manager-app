using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class MatchService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MatchService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<MatchDto>> GetMatchesAsync(Guid clubId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<MatchDto>>($"clubs/{clubId}/matches");
        return result ?? new List<MatchDto>();
    }

    public async Task<MatchDto?> GetMatchAsync(Guid clubId, Guid matchId)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<MatchDto>($"clubs/{clubId}/matches/{matchId}");
    }

    public async Task<List<MatchDto>> GetMyMatchesAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<MatchDto>>("matches/my");
        return result ?? new List<MatchDto>();
    }

    public async Task<MatchDto?> CreateMatchAsync(Guid clubId, CreateMatchRequest request)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"clubs/{clubId}/matches", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MatchDto>();
    }

    public async Task EnterResultAsync(Guid clubId, Guid matchId, EnterResultRequest request)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"clubs/{clubId}/matches/{matchId}/result", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<PlayerStatsDto>> GetPlayerStatsAsync(Guid clubId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<PlayerStatsDto>>($"clubs/{clubId}/matches/stats");
        return result ?? new List<PlayerStatsDto>();
    }
}
