using System.Collections.Generic;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IStudentAnswerService
    {
        Task<StudentAnswer?> GetAnswerByIdAsync(int studentAnswerId, string studentId); // Öğrencinin kendi cevabı
        Task<IEnumerable<StudentAnswer>> GetAnswersByScenarioAsync(int scenarioId, string studentId); // Öğrencinin belirli bir senaryoya tüm cevapları (eğer birden fazla izin veriliyorsa)
        Task<IEnumerable<StudentAnswer>> GetAllAnswersByStudentAsync(string studentId); // Öğrencinin tüm cevapları
        // Task CreateAnswerAsync(StudentAnswer answer); // Bu zaten StudentController.SubmitAnswer içinde yapılıyor olabilir
    }
}