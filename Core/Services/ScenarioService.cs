// Core/Services/ScenarioService.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Areas.Identity.Data; // DbContext namespace'i
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Factories; // ScenarioFactory için
using YetenekPusulasi.Core.Interfaces.Services;
// using YetenekPusulasi.Core.Events; // ScenarioAssignedToClassNotifier (Observer için sonraki adımda)

namespace YetenekPusulasi.Core.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly ScenarioFactory _scenarioFactory;
        // private readonly ScenarioAssignedToClassNotifier _scenarioAssignedNotifier; // Observer için sonra

        public ScenarioService(ApplicationDbContext context, ScenarioFactory scenarioFactory /*, ScenarioAssignedToClassNotifier scenarioAssignedNotifier */)
        {
            _context = context;
            _scenarioFactory = scenarioFactory;
            // _scenarioAssignedNotifier = scenarioAssignedNotifier; // Observer için sonra
        }

        public async Task<Scenario?> CreateScenarioAsync(string title, string description, ScenarioType type, string teacherId, int classroomId)
        {
            // Sınıfın var olup olmadığını ve öğretmenin o sınıfa yetkili olup olmadığını kontrol etmek iyi bir pratik olur.
            var classroomExists = await _context.Classrooms.AnyAsync(c => c.Id == classroomId && c.TeacherId == teacherId);
            if (!classroomExists)
            {
                // Hata yönetimi: Sınıf bulunamadı veya öğretmen yetkili değil.
                // Bir exception fırlatabilir veya null dönebilirsiniz.
                // Şimdilik null dönelim, Controller'da kontrol edilecek.
                return null;
            }

            var scenario = _scenarioFactory.Create(title, description, type, teacherId, classroomId);

            _context.Scenarios.Add(scenario);
            await _context.SaveChangesAsync();

            // <<< OBSERVER TETİKLEME BURAYA GELECEK (Sonraki Adım) >>>
            // if (scenario != null)
            // {
            //    var classroom = await _context.Classrooms.FindAsync(classroomId); // Notifier için classroom bilgisi
            //    if(classroom != null)
            //        await _scenarioAssignedNotifier.NotifyObserversAsync(scenario, classroom);
            // }

            return scenario;
        }

        public async Task<Scenario?> GetScenarioByIdAsync(int scenarioId)
        {
            return await _context.Scenarios
                .Include(s => s.Teacher) // İsteğe bağlı: Öğretmen bilgisini de getir
                .Include(s => s.Classroom) // İsteğe bağlı: Sınıf bilgisini de getir
                .FirstOrDefaultAsync(s => s.Id == scenarioId);
        }

        public async Task<IEnumerable<Scenario>> GetScenariosByClassroomAsync(int classroomId)
        {
            return await _context.Scenarios
                .Where(s => s.ClassroomId == classroomId)
                .Include(s => s.Teacher) // Oluşturan öğretmeni göster
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scenario>> GetScenariosByTeacherAsync(string teacherId)
        {
            return await _context.Scenarios
                .Where(s => s.TeacherId == teacherId)
                .Include(s => s.Classroom) // Hangi sınıfa ait olduğunu göster
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }
    }
}