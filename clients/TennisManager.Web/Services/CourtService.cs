using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class CourtService
{
    private readonly HttpClient _httpClient;

    public CourtService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<CourtDto>> GetCourtsAsync(Guid clubId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<CourtDto>>($"clubs/{clubId}/courts");
        return result ?? new List<CourtDto>();
    }

    public async Task<CourtDto?> GetCourtAsync(Guid clubId, Guid courtId)
        => await _httpClient.GetFromJsonAsync<CourtDto>($"clubs/{clubId}/courts/{courtId}");

    public async Task<CourtDto?> CreateCourtAsync(Guid clubId, CreateCourtRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/courts", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CourtDto>();
    }

    public async Task<CourtDto?> UpdateCourtAsync(Guid clubId, Guid courtId, UpdateCourtRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"clubs/{clubId}/courts/{courtId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CourtDto>();
    }

    public async Task DeleteCourtAsync(Guid clubId, Guid courtId)
    {
        var response = await _httpClient.DeleteAsync($"clubs/{clubId}/courts/{courtId}");
        response.EnsureSuccessStatusCode();
    }
}
