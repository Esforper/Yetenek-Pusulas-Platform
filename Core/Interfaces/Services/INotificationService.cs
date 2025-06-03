// Core/Interfaces/Services/INotificationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, string? url = null);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId); // Okunmamış bildirim sayısını almak için eklendi
    }
}