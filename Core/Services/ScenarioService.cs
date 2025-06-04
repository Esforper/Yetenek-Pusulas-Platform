// Core/Services/ScenarioService.cs
// ...
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Areas.Identity.Data;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Factories;
using YetenekPusulasi.Core.Interfaces.Entities;
using YetenekPusulasi.Core.Interfaces.Services; // IScenario için

namespace YetenekPusulasi.Core.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly ScenarioFactory _scenarioFactory;

        public ScenarioService(ApplicationDbContext context, ScenarioFactory scenarioFactory)
        {
            _context = context;
            _scenarioFactory = scenarioFactory;
        }

        public async Task<IScenario?> CreateScenarioAsync(string title, string description, ScenarioType type, string teacherId, int classroomId)
        {
            var classroomExists = await _context.Classrooms.AnyAsync(c => c.Id == classroomId && c.TeacherId == teacherId);
            if (!classroomExists)
            {
                return null;
            }

            // Factory artık IScenario döndürüyor
            IScenario scenario = _scenarioFactory.Create(title, description, type, teacherId, classroomId);

            // DbContext'e eklerken somut tipine cast etmemiz gerekebilir veya
            // DbContext'in abstract tipleri nasıl ele aldığına bağlı.
            // Genellikle TPH stratejisiyle sorun olmaz.
            if (scenario is Scenario concreteScenario) // Güvenli cast
            {
                _context.Scenarios.Add(concreteScenario); // Scenarios DbSet'i hala DbSet<Scenario>
                await _context.SaveChangesAsync();
                return concreteScenario; // veya scenario (IScenario olarak)
            }
            return null; // Eğer cast başarısız olursa (beklenmedik durum)
        }

        public async Task<IScenario?> GetScenarioByIdAsync(int scenarioId)
        {
            // Scenarios DbSet'i Scenario (abstract) tipinde olduğu için,
            // EF Core doğru alt tipi getirecektir (TPH sayesinde).
            return await _context.Scenarios
                .Include(s => s.Teacher)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(s => s.Id == scenarioId);
        }

        public async Task<IEnumerable<IScenario>> GetScenariosByClassroomAsync(int classroomId)
        {
            return await _context.Scenarios
                .Where(s => s.ClassroomId == classroomId)
                .Include(s => s.Teacher)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(); // Bu List<Scenario> döndürür, IScenario'ya otomatik cast olur.
        }

        public async Task<IEnumerable<IScenario>> GetScenariosByTeacherAsync(string teacherId)
        {
            return await _context.Scenarios
                .Where(s => s.TeacherId == teacherId)
                .Include(s => s.Classroom)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }
    }
}