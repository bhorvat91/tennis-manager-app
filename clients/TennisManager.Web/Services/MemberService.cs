using System.Net.Http.Json;
using TennisManager.Web.Models;

namespace TennisManager.Web.Services;

public class MemberService
{
    private readonly HttpClient _httpClient;

    public MemberService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ClubMemberDto>> GetMembersAsync(Guid clubId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ClubMemberDto>>($"clubs/{clubId}/members");
        return result ?? new List<ClubMemberDto>();
    }

    public async Task<List<ClubMemberDto>> GetPendingRequestsAsync(Guid clubId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ClubMemberDto>>($"clubs/{clubId}/members/pending");
        return result ?? new List<ClubMemberDto>();
    }

    public async Task RequestMembershipAsync(Guid clubId)
    {
        var response = await _httpClient.PostAsJsonAsync($"clubs/{clubId}/members/request", new RequestMembershipRequest());
        response.EnsureSuccessStatusCode();
    }

    public async Task ApproveMemberAsync(Guid clubId, Guid memberId)
    {
        var response = await _httpClient.PostAsync($"clubs/{clubId}/members/{memberId}/approve", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task RejectMemberAsync(Guid clubId, Guid memberId)
    {
        var response = await _httpClient.PostAsync($"clubs/{clubId}/members/{memberId}/reject", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRoleAsync(Guid clubId, Guid memberId, string role)
    {
        var response = await _httpClient.PutAsJsonAsync($"clubs/{clubId}/members/{memberId}/role", new UpdateRoleRequest { Role = role });
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveMemberAsync(Guid clubId, Guid memberId)
    {
        var response = await _httpClient.DeleteAsync($"clubs/{clubId}/members/{memberId}");
        response.EnsureSuccessStatusCode();
    }
}
