using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;

    public AuthService(IHttpClientFactory httpClientFactory, ITokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient("TennisManagerApi");
        var response = await client.PostAsJsonAsync("auth/login", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result != null)
            await _tokenService.SetTokenAsync(result.Token);
        return result;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var client = _httpClientFactory.CreateClient("TennisManagerApi");
        var response = await client.PostAsJsonAsync("auth/register", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result != null)
            await _tokenService.SetTokenAsync(result.Token);
        return result;
    }

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
