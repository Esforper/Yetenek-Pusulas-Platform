// Core/Events/IStudentJoinedObserver.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities; // Classroom, StudentClassroom için
using YetenekPusulasi.Data; // ApplicationUser için

namespace YetenekPusulasi.Core.Events
{
    public interface IStudentJoinedObserver
    {
        Task OnStudentJoinedClassroomAsync(StudentClassroom studentClassroom, ApplicationUser student, Classroom classroom);
    }
}