using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class CourtService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CourtService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<CourtDto>> GetCourtsAsync(Guid clubId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<CourtDto>>($"clubs/{clubId}/courts");
        return result ?? new List<CourtDto>();
    }

    public async Task<CourtDto?> GetCourtAsync(Guid courtId)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<CourtDto>($"courts/{courtId}");
    }

    public async Task<List<AvailabilitySlot>> GetAvailabilityAsync(Guid courtId, DateTime date)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<AvailabilitySlot>>($"courts/{courtId}/availability?date={date:yyyy-MM-dd}");
        return result ?? new List<AvailabilitySlot>();
    }
}
