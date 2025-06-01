// Core/Services/ScenarioOrchestrationService.cs (Servis + Strateji Kullanımı)
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Strategies;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Services
{
    public class ScenarioOrchestrationService
    {
        private readonly IScenarioRepository _scenarioRepository;
        private IScenarioGenerationStrategy _activeStrategy; // Çalışma zamanında değişebilir

        public ScenarioOrchestrationService(IScenarioRepository scenarioRepository, IScenarioGenerationStrategy defaultStrategy)
        {
            _scenarioRepository = scenarioRepository;
            _activeStrategy = defaultStrategy;
        }
        public void SetStrategy(IScenarioGenerationStrategy strategy) => _activeStrategy = strategy;

        public async Task<Scenario> CreateAndStoreScenarioAsync(string targetSkill, ScenarioDifficulty difficulty)
        {
            var scenario = await _activeStrategy.GenerateScenarioAsync(targetSkill, difficulty);
            await _scenarioRepository.AddAsync(scenario);
            return scenario;
        }
        public Task<Scenario> GetScenarioAsync(int id) => _scenarioRepository.GetByIdAsync(id);
    }
}
