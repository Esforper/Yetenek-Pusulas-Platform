// Core/Services/NotificationService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Areas.Identity.Data; // DbContext'inizin namespace'i

namespace YetenekPusulasi.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string message, string? url = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Url = url
                // IsRead ve CreatedDate constructor'da set ediliyor
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }


        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task MarkAllAsReadAsync(string userId)
        {
            var notificationsToUpdate = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (notificationsToUpdate.Any())
            {
                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}