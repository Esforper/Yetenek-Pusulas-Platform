// Core/Entities/Notification.cs
using System;
using YetenekPusulasi.Data; // ApplicationUser'ın namespace'i

namespace YetenekPusulasi.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Bildirimin gönderileceği kullanıcı (Öğretmen)
        public virtual ApplicationUser User { get; set; }

        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Url { get; set; } // Bildirime tıklandığında gidilecek sayfa (opsiyonel)

        public Notification()
        {
            IsRead = false;
            CreatedDate = DateTime.UtcNow;
        }
    }
}