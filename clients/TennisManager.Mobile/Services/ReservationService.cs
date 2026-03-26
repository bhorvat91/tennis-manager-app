using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class ReservationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReservationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<ReservationDto>> GetMyReservationsAsync()
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<ReservationDto>>("reservations/my");
        return result ?? new List<ReservationDto>();
    }

    public async Task<ReservationDto?> GetReservationAsync(Guid clubId, Guid reservationId)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<ReservationDto>($"clubs/{clubId}/reservations/{reservationId}");
    }

    public async Task<ReservationDto?> CreateReservationAsync(Guid clubId, CreateReservationRequest request)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"clubs/{clubId}/reservations", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReservationDto>();
    }

    public async Task CancelReservationAsync(Guid clubId, Guid reservationId)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"clubs/{clubId}/reservations/{reservationId}/cancel", new CancelReservationRequest());
        response.EnsureSuccessStatusCode();
    }
}
