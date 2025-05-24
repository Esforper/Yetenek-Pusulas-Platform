using YetenekPusulasi.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IClassroomService
    {
        Task<Classroom> CreateClassroomAsync(string name, string description, string teacherId);
        Task<Classroom> GetClassroomByIdAsync(int classroomId);
        Task<Classroom> GetClassroomByCodeAsync(string participationCode);
        Task<IEnumerable<Classroom>> GetClassroomsByTeacherAsync(string teacherId);
        Task<bool> AddStudentToClassroomAsync(string studentId, string participationCode);
        Task<IEnumerable<ApplicationUser>> GetStudentsInClassroomAsync(int classroomId);
        // ... diğer metotlar (Update, Delete vb.)
    }
}