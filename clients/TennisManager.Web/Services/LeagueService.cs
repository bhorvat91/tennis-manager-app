using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class LeagueService
{
    private readonly HttpClient _httpClient;

    public LeagueService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<LeagueDto>> GetLeaguesAsync(Guid clubId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<LeagueDto>>($"clubs/{clubId}/leagues");
        return result ?? new List<LeagueDto>();
    }

    public async Task<LeagueDto?> GetLeagueAsync(Guid clubId, Guid leagueId)
        => await _httpClient.GetFromJsonAsync<LeagueDto>($"clubs/{clubId}/leagues/{leagueId}");

    public async Task<LeagueDto?> CreateLeagueAsync(Guid clubId, CreateLeagueRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/leagues", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LeagueDto>();
    }

    public async Task<LeagueDto?> UpdateLeagueAsync(Guid clubId, Guid leagueId, UpdateLeagueRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"clubs/{clubId}/leagues/{leagueId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LeagueDto>();
    }

    public async Task DeleteLeagueAsync(Guid clubId, Guid leagueId)
    {
        var response = await _httpClient.DeleteAsync($"clubs/{clubId}/leagues/{leagueId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<LeagueParticipantDto>> GetParticipantsAsync(Guid clubId, Guid leagueId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<LeagueParticipantDto>>($"clubs/{clubId}/leagues/{leagueId}/participants");
        return result ?? new List<LeagueParticipantDto>();
    }

    public async Task JoinLeagueAsync(Guid clubId, Guid leagueId)
    {
        var response = await _httpClient.PostAsync($"clubs/{clubId}/leagues/{leagueId}/join", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task LeaveLeagueAsync(Guid clubId, Guid leagueId)
    {
        var response = await _httpClient.PostAsync($"clubs/{clubId}/leagues/{leagueId}/leave", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<LeagueMatchDto>> GetLeagueMatchesAsync(Guid clubId, Guid leagueId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<LeagueMatchDto>>($"clubs/{clubId}/leagues/{leagueId}/matches");
        return result ?? new List<LeagueMatchDto>();
    }

    public async Task<List<StandingDto>> GetStandingsAsync(Guid clubId, Guid leagueId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<StandingDto>>($"clubs/{clubId}/leagues/{leagueId}/standings");
        return result ?? new List<StandingDto>();
    }
}
