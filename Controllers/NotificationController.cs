// Controllers/NotificationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Services;


namespace YetenekPusulasi.WebApp.Controllers // Projenizin namespace'ine göre ayarlayın
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Bir bildirime tıklandığında bu action çalışacak
        public async Task<IActionResult> ViewNotification(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Veya login sayfasına yönlendir
            }

            // Önce bildirimi bul ve URL'sini al (varsa)
            var notification = (await _notificationService.GetUnreadNotificationsAsync(userId))
                                .FirstOrDefault(n => n.Id == id);
            // Eğer bildirim zaten okunmuşsa da URL'ye gitmesini sağlayabiliriz.
            // Bu durumda GetNotificationById gibi bir metod gerekebilir INotificationService'te.
            // Şimdilik sadece okunmamışlar üzerinden gidelim.

            string? redirectUrl = notification?.Url;

            // Bildirimi okundu olarak işaretle
            await _notificationService.MarkAsReadAsync(id, userId);

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }

            // Eğer URL yoksa veya geçersizse, öğretmenin ana paneline yönlendir
            return RedirectToAction("Dashboard", "Teacher");
        }

        // Tüm bildirimleri okundu olarak işaretle
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _notificationService.MarkAllAsReadAsync(userId);
            TempData["SuccessMessage"] = "Tüm bildirimler okundu olarak işaretlendi.";

            // Geldiği sayfaya geri dönmek daha iyi olabilir, ama basitlik için Dashboard'a yönlendirelim
            // string referer = Request.Headers["Referer"].ToString();
            // if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
            // {
            //    return Redirect(referer);
            // }
            return RedirectToAction("Dashboard", "Teacher");
        }
    }
}