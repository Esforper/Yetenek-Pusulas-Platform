// Core/Interfaces/Services/IScenarioService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IScenarioService
    {
        // ScenarioCreateDto gibi bir DTO kullanılabilir veya doğrudan parametreler
        Task<Scenario?> CreateScenarioAsync(string title, string description, ScenarioType type, string teacherId, int classroomId);
        Task<Scenario?> GetScenarioByIdAsync(int scenarioId);
        Task<IEnumerable<Scenario>> GetScenariosByClassroomAsync(int classroomId);
        Task<IEnumerable<Scenario>> GetScenariosByTeacherAsync(string teacherId); // Öğretmenin oluşturduğu tüm senaryolar
        // Task<Scenario> UpdateScenarioAsync(Scenario scenario); // Opsiyonel
        // Task<bool> DeleteScenarioAsync(int scenarioId); // Opsiyonel
    }
}