using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class ReservationService
{
    private readonly HttpClient _httpClient;

    public ReservationService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ReservationDto>> GetReservationsAsync(Guid clubId, DateTime? date = null)
    {
        var url = $"clubs/{clubId}/reservations";
        if (date.HasValue)
            url += $"?date={date.Value:yyyy-MM-dd}";
        var result = await _httpClient.GetFromJsonAsync<List<ReservationDto>>(url);
        return result ?? new List<ReservationDto>();
    }

    public async Task<ReservationDto?> GetReservationAsync(Guid clubId, Guid reservationId)
        => await _httpClient.GetFromJsonAsync<ReservationDto>($"clubs/{clubId}/reservations/{reservationId}");

    public async Task<ReservationDto?> CreateReservationAsync(Guid clubId, CreateReservationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/reservations", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReservationDto>();
    }

    public async Task CancelReservationAsync(Guid clubId, Guid reservationId)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/reservations/{reservationId}/cancel", new CancelReservationRequest());
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ReservationDto>> GetMyReservationsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ReservationDto>>("reservations/my");
        return result ?? new List<ReservationDto>();
    }

    public async Task<List<AvailabilitySlot>> GetAvailabilityAsync(Guid courtId, DateTime date)
    {
        var result = await _httpClient.GetFromJsonAsync<List<AvailabilitySlot>>($"courts/{courtId}/availability?date={date:yyyy-MM-dd}");
        return result ?? new List<AvailabilitySlot>();
    }
}
