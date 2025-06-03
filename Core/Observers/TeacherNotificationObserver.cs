// Core/Observers/TeacherNotificationObserver.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Events;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Data;
using Microsoft.Extensions.Logging; // Loglama için eklendi

namespace YetenekPusulasi.Core.Observers
{
    public class TeacherNotificationObserver : IStudentJoinedObserver
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<TeacherNotificationObserver> _logger; // Hata ayıklama için

        public TeacherNotificationObserver(INotificationService notificationService, ILogger<TeacherNotificationObserver> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task OnStudentJoinedClassroomAsync(StudentClassroom studentClassroom, ApplicationUser student, Classroom classroom)
        {
            if (string.IsNullOrEmpty(classroom.TeacherId))
            {
                _logger.LogWarning("Sınıf ID {ClassroomId} için öğretmen ID'si bulunamadı. Bildirim oluşturulamadı.", classroom.Id);
                return;
            }

            if (student == null || string.IsNullOrEmpty(student.UserName))
            {
                _logger.LogWarning("Sınıfa katılan öğrenci bilgileri eksik. Öğrenci ID: {StudentId}. Bildirim oluşturulamadı.", studentClassroom.StudentId);
                return;
            }

            string message = $"{student.UserName} adlı öğrenci '{classroom.Name}' sınıfınıza katıldı.";
            // Basit bir URL, gerçek URL oluşturma daha sonra eklenebilir.
            string? notificationUrl = $"/Teacher/ClassroomDetails/{classroom.Id}";

            try
            {
                await _notificationService.CreateNotificationAsync(classroom.TeacherId, message, notificationUrl);
                _logger.LogInformation("Öğretmen ID {TeacherId} için '{ClassroomName}' sınıfına katılım bildirimi oluşturuldu.", classroom.TeacherId, classroom.Name);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Öğretmen için bildirim oluşturulurken hata oluştu. Öğretmen ID: {TeacherId}, Sınıf ID: {ClassroomId}", classroom.TeacherId, classroom.Id);
            }
        }
    }
}