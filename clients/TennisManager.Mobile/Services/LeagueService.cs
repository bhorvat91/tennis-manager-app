using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class LeagueService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeagueService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<LeagueDto>> GetLeaguesAsync(Guid clubId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<LeagueDto>>($"clubs/{clubId}/leagues");
        return result ?? new List<LeagueDto>();
    }

    public async Task<List<StandingDto>> GetStandingsAsync(Guid leagueId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<StandingDto>>($"leagues/{leagueId}/standings");
        return result ?? new List<StandingDto>();
    }
}
