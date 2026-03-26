using System.Net.Http.Json;
using TennisManager.Mobile.Models;

namespace TennisManager.Mobile.Services;

public class MemberService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MemberService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TennisManagerApi");

    public async Task<List<ClubMemberDto>> GetMembersAsync(Guid clubId)
    {
        var client = CreateClient();
        var result = await client.GetFromJsonAsync<List<ClubMemberDto>>($"clubs/{clubId}/members");
        return result ?? new List<ClubMemberDto>();
    }
}
