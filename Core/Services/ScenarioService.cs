using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.ValueObjects;
using System.Threading.Tasks;
using System.Collections.Generic; // IEnumerable için

namespace YetenekPusulasi.Core.Services
{
    public class ScenarioService
    {
        private readonly IScenarioRepository _scenarioRepository;
        private IScenarioGenerationStrategy _generationStrategy; // Aktif strateji

        public ScenarioService(IScenarioRepository scenarioRepository, IScenarioGenerationStrategy defaultStrategy)
        {
            _scenarioRepository = scenarioRepository;
            _generationStrategy = defaultStrategy; // Başlangıç stratejisi DI ile gelir
        }

        // Çalışma zamanında stratejiyi değiştirmek için bir metot
        public void SetScenarioGenerationStrategy(IScenarioGenerationStrategy strategy)
        {
            _generationStrategy = strategy;
        }

        public async Task<Scenario> CreatePersonalizedScenarioAsync(StudentProfile profile)
        {
            var scenario = await _generationStrategy.GenerateScenarioAsync(profile);
            await _scenarioRepository.AddAsync(scenario); // Üretilen senaryoyu kaydet
            return scenario;
        }

        public Task<Scenario> GetScenarioByIdAsync(int id)
        {
            return _scenarioRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Scenario>> GetAllScenariosAsync()
        {
            return _scenarioRepository.GetAllAsync();
        }

        // Basit CRUD için (Repository doğrudan kullanılabilir veya Service üzerinden sarmalanabilir)
        public async Task AddScenarioManuallyAsync(Scenario scenario)
        {
            // Burada ek iş kuralları, validasyonlar vb. olabilir
            await _scenarioRepository.AddAsync(scenario);
        }
    }
}