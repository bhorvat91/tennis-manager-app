using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class MatchService
{
    private readonly HttpClient _httpClient;

    public MatchService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<MatchDto>> GetMatchesAsync(Guid clubId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<MatchDto>>($"clubs/{clubId}/matches");
        return result ?? new List<MatchDto>();
    }

    public async Task<MatchDto?> GetMatchAsync(Guid clubId, Guid matchId)
        => await _httpClient.GetFromJsonAsync<MatchDto>($"clubs/{clubId}/matches/{matchId}");

    public async Task<MatchDto?> CreateMatchAsync(Guid clubId, CreateMatchRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/matches", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MatchDto>();
    }

    public async Task DeleteMatchAsync(Guid clubId, Guid matchId)
    {
        var response = await _httpClient.DeleteAsync($"clubs/{clubId}/matches/{matchId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task EnterResultAsync(Guid clubId, Guid matchId, EnterResultRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/matches/{matchId}/result", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<MatchDto>> GetMyMatchesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<MatchDto>>("matches/my");
        return result ?? new List<MatchDto>();
    }

    public async Task<PlayerStatsDto?> GetPlayerStatsAsync(Guid clubId, Guid userId)
        => await _httpClient.GetFromJsonAsync<PlayerStatsDto>($"clubs/{clubId}/stats/{userId}");
}
