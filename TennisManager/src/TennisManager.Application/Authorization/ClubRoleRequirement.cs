using Microsoft.AspNetCore.Authorization;
using TennisManager.Domain.Enums;

namespace TennisManager.Application.Authorization;

public class ClubRoleRequirement : IAuthorizationRequirement
{
    public ClubRole[] AllowedRoles { get; }

    public ClubRoleRequirement(params ClubRole[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
