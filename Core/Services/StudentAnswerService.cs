using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Areas.Identity.Data; // DbContext namespace'i
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Services;

namespace YetenekPusulasi.Core.Services
{
    public class StudentAnswerService : IStudentAnswerService
    {
        private readonly ApplicationDbContext _context;

        public StudentAnswerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAnswer?> GetAnswerByIdAsync(int studentAnswerId, string studentId)
        {
            return await _context.StudentAnswers
                .Include(sa => sa.Scenario) // Senaryo bilgisini de al
                .Include(sa => sa.AnalysisResult) // Analiz sonucunu da al
                .FirstOrDefaultAsync(sa => sa.Id == studentAnswerId && sa.StudentId == studentId);
        }

        public async Task<IEnumerable<StudentAnswer>> GetAnswersByScenarioAsync(int scenarioId, string studentId)
        {
            return await _context.StudentAnswers
                .Where(sa => sa.ScenarioId == scenarioId && sa.StudentId == studentId)
                .Include(sa => sa.Scenario)
                .Include(sa => sa.AnalysisResult)
                .OrderByDescending(sa => sa.SubmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentAnswer>> GetAllAnswersByStudentAsync(string studentId)
        {
            return await _context.StudentAnswers
                .Where(sa => sa.StudentId == studentId)
                .Include(sa => sa.Scenario) // Her cevabın hangi senaryoya ait olduğunu bilmek için
                .Include(sa => sa.AnalysisResult)
                .OrderByDescending(sa => sa.SubmissionDate)
                .ToListAsync();
        }
    }
}