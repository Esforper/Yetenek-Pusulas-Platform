using YetenekPusulasi.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetenekPusulasi.Models;
using YetenekPusulasi.Data;

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

        Task<IEnumerable<Classroom>> GetClassroomsByStudentAsync(string studentId);
        Task<bool> IsStudentEnrolledAsync(string studentId, int classroomId); // Opsiyonel: Yetki kontrolü için
    }
}