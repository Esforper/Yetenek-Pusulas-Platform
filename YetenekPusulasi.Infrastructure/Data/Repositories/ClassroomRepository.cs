// Infrastructure/Data/Repositories/ClassroomRepository.cs
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Linq; // FirstOrDefaultAsync için

namespace YetenekPusulasi.Infrastructure.Data.Repositories
{
    public class ClassroomRepository : EfRepository<Classroom>, IClassroomRepository
    {
        public ClassroomRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Classroom> GetByParticipationCodeAsync(string code)
        {
            return await _dbContext.Classrooms
                .Include(c => c.Teacher) // Öğretmen bilgisini de çekelim
                .Include(c => c.StudentClassrooms) // Öğrenci-Sınıf ilişkilerini de çekelim (opsiyonel)
                    // .ThenInclude(sc => sc.Student) // Eğer öğrenci detayları da gerekirse
                .FirstOrDefaultAsync(c => c.ParticipationCode == code);
        }

        public async Task<Classroom> GetByIdWithStudentsAsync(int id)
        {
            return await _dbContext.Classrooms
                .Include(c => c.Teacher)
                .Include(c => c.StudentClassrooms)
                    .ThenInclude(sc => sc.Student) // Öğrencilerin detaylarını da çekmek için
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // EfRepository<Classroom>'dan gelen genel CRUD metotları zaten mevcut olacak.
        // Override etmek veya özel implementasyonlar eklemek isterseniz buraya yazabilirsiniz.
        // Örneğin, TeacherId'ye göre sınıfları getiren bir metot eklenebilir:
        // public async Task<IEnumerable<Classroom>> GetClassroomsByTeacherIdAsync(string teacherId)
        // {
        //     return await _dbContext.Classrooms
        //         .Where(c => c.TeacherId == teacherId)
        //         .ToListAsync();
        // }
    }
}
