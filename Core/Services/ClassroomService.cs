using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Data; // ApplicationDbContext için
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Models; // ApplicationUser için

namespace YetenekPusulasi.Core.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly ApplicationDbContext _context;

        public ClassroomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Classroom> CreateClassroomAsync(string name, string description, string teacherId)
        {
            var classroom = new Classroom
            {
                Name = name,
                Description = description,
                TeacherId = teacherId,
                ParticipationCode = GenerateUniqueParticipationCode() // Helper metot
            };
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();
            return classroom;
        }

        public async Task<bool> AddStudentToClassroomAsync(string studentId, string participationCode)
        {
            var classroom = await _context.Classrooms
                                      .FirstOrDefaultAsync(c => c.ParticipationCode == participationCode);
            if (classroom == null) return false; // Sınıf bulunamadı

            var student = await _context.Users.FindAsync(studentId);
            if (student == null) return false; // Öğrenci bulunamadı

            // Öğrenci zaten bu sınıfta mı kontrolü
            bool alreadyEnrolled = await _context.StudentClassrooms
                .AnyAsync(sc => sc.StudentId == studentId && sc.ClassroomId == classroom.Id);
            if (alreadyEnrolled) return true; // Zaten kayıtlı, başarılı sayılabilir veya farklı bir dönüş

            var studentClassroom = new StudentClassroom
            {
                StudentId = studentId,
                ClassroomId = classroom.Id,
                JoinedDate = DateTime.UtcNow
            };
            _context.StudentClassrooms.Add(studentClassroom);
            await _context.SaveChangesAsync();
            return true;
        }
        public Task<Classroom> GetClassroomByIdAsync(int classroomId) =>
             _context.Classrooms.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.Id == classroomId);

        public Task<Classroom> GetClassroomByCodeAsync(string participationCode) =>
            _context.Classrooms.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.ParticipationCode == participationCode);


        public async Task<IEnumerable<Classroom>> GetClassroomsByTeacherAsync(string teacherId) =>
            await _context.Classrooms.Where(c => c.TeacherId == teacherId).ToListAsync();

        public async Task<IEnumerable<ApplicationUser>> GetStudentsInClassroomAsync(int classroomId) =>
             await _context.StudentClassrooms
                           .Where(sc => sc.ClassroomId == classroomId)
                           .Select(sc => sc.Student)
                           .ToListAsync();


        private string GenerateUniqueParticipationCode(int length = 6)
        {
            // Basit bir kod üretme mekanizması, daha robust bir çözüm gerekebilir.
            // Çakışma kontrolü yapılmalı (veritabanında var mı diye döngü içinde kontrol edilebilir)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;
            do
            {
                code = new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (_context.Classrooms.Any(c => c.ParticipationCode == code)); // Çakışma kontrolü
            return code;
        }
    }
}