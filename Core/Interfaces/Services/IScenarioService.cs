// Core/Interfaces/Services/IScenarioService.cs
// ...
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IScenarioService
    {
        Task<IScenario?> CreateScenarioAsync(string title, string description, ScenarioType type, string teacherId, int classroomId,
        string? teacherProvidedInitialPrompt, bool generateInitialPromptWithAI);
        Task<IScenario?> GetScenarioByIdAsync(int scenarioId);
        Task<IEnumerable<IScenario>> GetScenariosByClassroomAsync(int classroomId);
        Task<IEnumerable<IScenario>> GetScenariosByTeacherAsync(string teacherId);
    }
}