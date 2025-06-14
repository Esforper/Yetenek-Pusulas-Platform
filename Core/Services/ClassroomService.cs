using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Data; // ApplicationDbContext için
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Models;
using YetenekPusulasi.Core.Events;
using YetenekPusulasi.Areas.Identity.Data; // ApplicationUser için

namespace YetenekPusulasi.Core.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentJoinedClassroomNotifier _studentJoinedNotifier;

        public ClassroomService(ApplicationDbContext context, StudentJoinedClassroomNotifier studentJoinedNotifier)
        {
            _context = context;
            _studentJoinedNotifier = studentJoinedNotifier; // <<< YENİ EKLEME
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


            // Observer'lara bildirim gönderme
            if (student is ApplicationUser appUser) // Tip kontrolü ve cast
            {
                await _studentJoinedNotifier.NotifyObserversAsync(studentClassroom, appUser, classroom);
            }
            else
            {
                // Hata loglama veya farklı bir işlem
                // Bu durumun olmaması beklenir eğer Users DbSet'i doğru konfigüre edilmişse.
                Console.WriteLine($"Hata: Student ID {studentId} ApplicationUser tipinde değil.");
            }


            return true;
        }


        

        public async Task<IEnumerable<Classroom>> GetClassroomsByTeacherAsync(string teacherId)
        {
            return await _context.Classrooms
                .Where(c => c.TeacherId == teacherId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetStudentsInClassroomAsync(int classroomId)
        {
            return await _context.StudentClassrooms
                           .Where(sc => sc.ClassroomId == classroomId)
                           .Select(sc => sc.Student)
                           .ToListAsync();
        }

        // YENİ METOT IMPLEMENTASYONU
        public async Task<IEnumerable<Classroom>> GetClassroomsByStudentAsync(string studentId)
        {
            return await _context.StudentClassrooms
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Classroom) // Sınıf bilgilerini de çek
                    .ThenInclude(c => c.Teacher) // Sınıfın öğretmenini de çek (opsiyonel)
                .Select(sc => sc.Classroom)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // OPSİYONEL YENİ METOT IMPLEMENTASYONU
        public async Task<bool> IsStudentEnrolledAsync(string studentId, int classroomId)
        {
            return await _context.StudentClassrooms
                .AnyAsync(sc => sc.StudentId == studentId && sc.ClassroomId == classroomId);
        }





        public Task<Classroom> GetClassroomByIdAsync(int classroomId) =>
             _context.Classrooms.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.Id == classroomId);

        public Task<Classroom> GetClassroomByCodeAsync(string participationCode) =>
            _context.Classrooms.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.ParticipationCode == participationCode);

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