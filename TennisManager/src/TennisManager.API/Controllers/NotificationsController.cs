using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Services;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    /// <summary>Get paginated list of notifications for the current user.</summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value, page, pageSize);
        var unreadCount = await _notificationService.GetUnreadCountAsync(userId.Value);
        var notificationList = notifications.ToList();

        Response.Headers["X-Unread-Count"] = unreadCount.ToString();

        return Ok(new NotificationListResponse
        {
            Notifications = notificationList.Select(MapToResponse),
            UnreadCount = unreadCount,
            TotalCount = notificationList.Count,
            Page = page,
            PageSize = pageSize
        });
    }

    /// <summary>Get unread notification count for the current user.</summary>
    [Authorize]
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync(userId.Value);
        return Ok(new { unreadCount = count });
    }

    /// <summary>Mark a notification as read. Must belong to the current user.</summary>
    [Authorize]
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        await _notificationService.MarkAsReadAsync(id, userId.Value);
        return NoContent();
    }

    /// <summary>Mark all notifications as read for the current user.</summary>
    [Authorize]
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId.Value);
        return NoContent();
    }

    /// <summary>Delete a notification. Must belong to the current user.</summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        await _notificationService.DeleteNotificationAsync(id, userId.Value);
        return NoContent();
    }

    private static NotificationResponse MapToResponse(Notification n) => new()
    {
        Id = n.Id,
        Type = n.Type,
        Channel = n.Channel,
        Status = n.Status,
        Title = n.Title,
        Body = n.Body,
        Data = n.Data,
        ScheduledAt = n.ScheduledAt,
        SentAt = n.SentAt,
        ReadAt = n.ReadAt,
        CreatedAt = n.CreatedAt
    };
}
