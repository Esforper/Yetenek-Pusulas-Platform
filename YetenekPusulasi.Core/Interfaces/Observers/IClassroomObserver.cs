// Core/Interfaces/Observers/IClassroomObserver.cs (OBSERVER)
using YetenekPusulasi.Core.Entities;
namespace YetenekPusulasi.Core.Interfaces.Observers
{
    public interface IClassroomObserver
    {
        void OnClassroomCreated(Classroom classroom);
        void OnStudentJoined(Classroom classroom, ApplicationUser student);
    }
}