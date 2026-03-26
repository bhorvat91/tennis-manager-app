using Microsoft.EntityFrameworkCore;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Enums;
using TennisManager.Domain.Interfaces.Repositories;
using TennisManager.Infrastructure.Persistence;

namespace TennisManager.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _db;

    public NotificationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _db.Notifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _db.Notifications
            .CountAsync(n => n.UserId == userId
                && n.Channel == NotificationChannel.InApp
                && n.ReadAt == null);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.UtcNow;
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return notification;
    }

    public async Task UpdateAsync(Notification notification)
    {
        _db.Notifications.Update(notification);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification is not null)
        {
            _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification is not null)
        {
            notification.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _db.Notifications
            .Where(n => n.UserId == userId
                && n.Channel == NotificationChannel.InApp
                && n.ReadAt == null)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.ReadAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetPendingScheduledAsync(DateTime beforeTime)
    {
        return await _db.Notifications
            .Where(n => n.Status == NotificationStatus.Pending
                && n.ScheduledAt != null
                && n.ScheduledAt <= beforeTime)
            .ToListAsync();
    }
}
