using System.Net.Http.Json;
using Blazored.LocalStorage;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<string?> GetTokenAsync() => await _localStorage.GetItemAsync<string>(TokenKey);

    public async Task SetTokenAsync(string token) => await _localStorage.SetItemAsync(TokenKey, token);

    public async Task RemoveTokenAsync() => await _localStorage.RemoveItemAsync(TokenKey);

    public async Task LogoutAsync()
    {
        await RemoveTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
