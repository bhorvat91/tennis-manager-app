using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Interfaces.Repositories;

namespace TennisManager.Application.Authorization;

public class ClubRoleHandler : AuthorizationHandler<ClubRoleRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IClubMemberRepository _clubMemberRepository;
    private readonly ICurrentUserService _currentUserService;

    public ClubRoleHandler(
        IHttpContextAccessor httpContextAccessor,
        IClubMemberRepository clubMemberRepository,
        ICurrentUserService currentUserService)
    {
        _httpContextAccessor = httpContextAccessor;
        _clubMemberRepository = clubMemberRepository;
        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ClubRoleRequirement requirement)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return;

        var routeValues = _httpContextAccessor.HttpContext?.Request.RouteValues;
        if (routeValues is null)
            return;

        // Try "clubId" first (MembersController, CourtsController), then "id" (ClubsController)
        if (!routeValues.TryGetValue("clubId", out var clubIdValue) || clubIdValue is null)
            routeValues.TryGetValue("id", out clubIdValue);

        if (clubIdValue is null)
            return;

        if (!Guid.TryParse(clubIdValue.ToString(), out var clubId))
            return;

        var member = await _clubMemberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (member is null)
            return;

        if (requirement.AllowedRoles.Contains(member.Role) &&
            member.Status == Domain.Enums.MemberStatus.Approved)
        {
            context.Succeed(requirement);
        }
    }
}
