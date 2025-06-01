// Core/Services/ClassroomManagementService.cs (Servis + Observer Kullanımı)
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Services
{
    public class ClassroomManagementService
    {
        private readonly IClassroomRepository _classroomRepository;
        private readonly IRepository<ApplicationUser> _userRepository; // Öğrenci bulmak için
        private readonly List<IClassroomObserver> _observers = new List<IClassroomObserver>();

        public ClassroomManagementService(IClassroomRepository classroomRepository, IRepository<ApplicationUser> userRepository)
        {
            _classroomRepository = classroomRepository;
            _userRepository = userRepository;
        }
        public void Subscribe(IClassroomObserver observer) => _observers.Add(observer);
        public void Unsubscribe(IClassroomObserver observer) => _observers.Remove(observer);

        private void NotifyClassroomCreated(Classroom classroom) => _observers.ForEach(o => o.OnClassroomCreated(classroom));
        private void NotifyStudentJoined(Classroom classroom, ApplicationUser student) => _observers.ForEach(o => o.OnStudentJoined(classroom, student));

        public async Task<Classroom> CreateClassroomAsync(string name, string teacherId)
        {
            // Basit katılım kodu üretimi (daha robust bir çözüm gerekebilir)
            string code;
            do {
                code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            } while (await _classroomRepository.GetByParticipationCodeAsync(code) != null);

            var classroom = new Classroom { Name = name, TeacherId = teacherId, ParticipationCode = code };
            await _classroomRepository.AddAsync(classroom);
            NotifyClassroomCreated(classroom);
            return classroom;
        }

        public async Task<bool> JoinStudentToClassroomAsync(string studentId, string participationCode)
        {
            var classroom = await _classroomRepository.GetByParticipationCodeAsync(participationCode);
            var student = await _userRepository.GetByIdAsync(studentId); // IRepository<ApplicationUser> üzerinden
            if (classroom == null || student == null) return false;

            // Zaten kayıtlı mı kontrolü (basitleştirilmiş)
            if (classroom.StudentClassrooms.Any(sc => sc.StudentId == studentId)) return true;

            classroom.StudentClassrooms.Add(new StudentClassroom { StudentId = studentId, ClassroomId = classroom.Id });
            await _classroomRepository.UpdateAsync(classroom); // Veya ayrı StudentClassroomRepo
            NotifyStudentJoined(classroom, student);
            return true;
        }
        private readonly IEnumerable<IClassroomObserver> _observers; // IEnumerable olarak al

        public ClassroomManagementService(
        IClassroomRepository classroomRepository,
        IRepository<ApplicationUser> userRepository, // veya IUserRepository
        IEnumerable<IClassroomObserver> observers) // DI ile tüm observer'lar gelir
    {
        _classroomRepository = classroomRepository;
        _userRepository = userRepository;
        _observers = observers ?? new List<IClassroomObserver>(); // Null gelirse boş liste
    }
    private void NotifyClassroomCreated(Classroom classroom)
    {
        foreach (var observer in _observers)
        {
            observer.OnClassroomCreated(classroom);
        }
    }
    private void NotifyStudentJoined(Classroom classroom, ApplicationUser student)
    {
        foreach (var observer in _observers)
        {
            observer.OnStudentJoined(classroom, student);
        }
    }
    // ... (diğer metotlar aynı)
}
    }
}