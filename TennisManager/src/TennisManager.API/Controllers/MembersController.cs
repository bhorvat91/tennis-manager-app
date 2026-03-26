using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Domain.Interfaces.Services;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/clubs/{clubId:guid}/members")]
public class MembersController : ControllerBase
{
    private readonly IClubMemberRepository _memberRepository;
    private readonly IClubRepository _clubRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public MembersController(
        IClubMemberRepository memberRepository,
        IClubRepository clubRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _memberRepository = memberRepository;
        _clubRepository = clubRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    /// <summary>Get all members of a club. Admin or Coach only.</summary>
    [Authorize(Policy = "ClubCoachOrAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetMembers(Guid clubId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var members = await _memberRepository.GetByClubIdAsync(clubId);
        return Ok(members.Select(MapToResponse));
    }

    /// <summary>Submit a membership request to a club.</summary>
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> RequestMembership(Guid clubId)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var existing = await _memberRepository.GetByClubAndUserAsync(clubId, userId.Value);
        if (existing is not null)
            return Conflict(new { message = "You already have a membership request for this club." });

        var member = new ClubMember
        {
            ClubId = clubId,
            UserId = userId.Value,
            Role = ClubRole.Player,
            Status = MemberStatus.Pending
        };

        var created = await _memberRepository.CreateAsync(member);
        return CreatedAtAction(nameof(GetMember), new { clubId, userId = userId.Value }, MapToResponse(created));
    }

    /// <summary>Get pending membership requests. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(Guid clubId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var pending = await _memberRepository.GetPendingByClubIdAsync(clubId);
        return Ok(pending.Select(MapToResponse));
    }

    /// <summary>Approve a membership request. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{userId:guid}/approve")]
    public async Task<IActionResult> ApproveMember(Guid clubId, Guid userId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId);
        if (member is null)
            return NotFound(new { message = "Membership request not found." });

        member.Status = MemberStatus.Approved;
        member.JoinedAt = DateTime.UtcNow;
        await _memberRepository.UpdateAsync(member);

        await _notificationService.SendMembershipApprovedAsync(userId, clubId, club.Name);

        return Ok(MapToResponse(member));
    }

    /// <summary>Reject a membership request. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{userId:guid}/reject")]
    public async Task<IActionResult> RejectMember(Guid clubId, Guid userId)
    {
        var club = await _clubRepository.GetByIdAsync(clubId);
        if (club is null)
            return NotFound(new { message = $"Club {clubId} not found." });

        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId);
        if (member is null)
            return NotFound(new { message = "Membership request not found." });

        member.Status = MemberStatus.Rejected;
        await _memberRepository.UpdateAsync(member);

        await _notificationService.SendMembershipRejectedAsync(userId, clubId, club.Name);

        return Ok(MapToResponse(member));
    }

    /// <summary>Update a member's role. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpPut("{userId:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid clubId, Guid userId, [FromBody] UpdateMemberRoleRequest request)
    {
        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId);
        if (member is null)
            return NotFound(new { message = "Member not found." });

        member.Role = request.Role;
        await _memberRepository.UpdateAsync(member);

        return Ok(MapToResponse(member));
    }

    /// <summary>Remove a member from the club. Admin only.</summary>
    [Authorize(Policy = "ClubAdmin")]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid clubId, Guid userId)
    {
        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId);
        if (member is null)
            return NotFound(new { message = "Member not found." });

        await _memberRepository.DeleteAsync(clubId, userId);
        return NoContent();
    }

    /// <summary>Get a specific member's details. Admin or Coach only.</summary>
    [Authorize(Policy = "ClubCoachOrAdmin")]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetMember(Guid clubId, Guid userId)
    {
        var member = await _memberRepository.GetByClubAndUserAsync(clubId, userId);
        if (member is null)
            return NotFound(new { message = "Member not found." });

        return Ok(MapToResponse(member));
    }

    private static ClubMemberResponse MapToResponse(ClubMember m) => new()
    {
        Id = m.Id,
        ClubId = m.ClubId,
        UserId = m.UserId,
        UserEmail = m.User?.Email ?? string.Empty,
        UserFirstName = m.User?.FirstName ?? string.Empty,
        UserLastName = m.User?.LastName ?? string.Empty,
        Role = m.Role,
        Status = m.Status,
        JoinedAt = m.JoinedAt,
        CreatedAt = m.CreatedAt
    };
}
