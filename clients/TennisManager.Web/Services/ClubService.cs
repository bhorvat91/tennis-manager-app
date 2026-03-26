using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class ClubService
{
    private readonly HttpClient _httpClient;

    public ClubService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ClubDto>> GetClubsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ClubDto>>("clubs");
        return result ?? new List<ClubDto>();
    }

    public async Task<ClubDto?> GetClubAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<ClubDto>($"clubs/{id}");

    public async Task<ClubDto?> CreateClubAsync(CreateClubRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("clubs", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClubDto>();
    }

    public async Task<ClubDto?> UpdateClubAsync(Guid id, UpdateClubRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"clubs/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClubDto>();
    }

    public async Task DeleteClubAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"clubs/{id}");
        response.EnsureSuccessStatusCode();
    }
}
