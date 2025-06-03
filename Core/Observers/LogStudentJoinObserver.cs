// Core/Observers/LogStudentJoinObserver.cs
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Events;
using YetenekPusulasi.Data;

namespace YetenekPusulasi.Core.Observers
{
    public class LogStudentJoinObserver : IStudentJoinedObserver
    {
        private readonly ILogger<LogStudentJoinObserver> _logger;

        public LogStudentJoinObserver(ILogger<LogStudentJoinObserver> logger)
        {
            _logger = logger;
        }

        public Task OnStudentJoinedClassroomAsync(StudentClassroom studentClassroom, ApplicationUser student, Classroom classroom)
        {
            _logger.LogInformation(
                "Öğrenci Sınıfa Katıldı: Öğrenci ID '{StudentId}' ({StudentUserName}), Sınıf ID '{ClassroomId}' ({ClassroomName}), Tarih: {JoinedDate}",
                student.Id,
                student.UserName,
                classroom.Id,
                classroom.Name,
                studentClassroom.JoinedDate
            );
            return Task.CompletedTask;
        }
    }
}