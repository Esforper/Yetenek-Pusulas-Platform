// Core/Events/StudentJoinedClassroomNotifier.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Data;

namespace YetenekPusulasi.Core.Events
{
    public class StudentJoinedClassroomNotifier
    {
        // Observer'ları DI ile almak daha esnek olabilir, ama basitlik için AddObserver metodu
        private readonly IEnumerable<IStudentJoinedObserver> _observers;

        // DI ile observer'ları alacak constructor
        public StudentJoinedClassroomNotifier(IEnumerable<IStudentJoinedObserver> observers)
        {
            _observers = observers ?? Enumerable.Empty<IStudentJoinedObserver>();
        }

        public async Task NotifyObserversAsync(StudentClassroom studentClassroom, ApplicationUser student, Classroom classroom)
        {
            // Paralel çalıştırmak yerine sıralı çalıştırmak daha güvenli olabilir bazı senaryolarda
            // Veya Task.WhenAll kullanılarak hepsi beklenebilir.
            foreach (var observer in _observers)
            {
                await observer.OnStudentJoinedClassroomAsync(studentClassroom, student, classroom);
            }
        }
    }
}