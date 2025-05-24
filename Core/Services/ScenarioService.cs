// Core/Services/ScenarioService.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Core.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using YetenekPusulasi.Core.Strategies; // ArgumentNullException için

namespace YetenekPusulasi.Core.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IScenarioCategoryRepository _categoryRepository;
        // Stratejileri DI ile almak için:
        private readonly IEnumerable<IScenarioGenerationStrategy> _generationStrategies;
        // Veya tek tek enjekte etmek:
        // private readonly RuleBasedScenarioStrategy _ruleStrategy;
        // private readonly AIModelScenarioStrategy _aiStrategy;

        public ScenarioService(
            IScenarioRepository scenarioRepository,
            IScenarioCategoryRepository categoryRepository,
            IEnumerable<IScenarioGenerationStrategy> generationStrategies) // Tüm stratejileri enjekte et
        {
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _generationStrategies = generationStrategies ?? throw new ArgumentNullException(nameof(generationStrategies));
        }

        // Senaryo CRUD
        public Task<Scenario> GetScenarioByIdAsync(int id) => _scenarioRepository.GetByIdAsync(id);
        public Task<IEnumerable<Scenario>> GetAllScenariosAsync() => _scenarioRepository.GetAllAsync();
        public async Task<Scenario> CreateScenarioAsync(Scenario scenario)
        {
            // Ek validasyonlar
            await _scenarioRepository.AddAsync(scenario);
            return scenario;
        }
        public Task UpdateScenarioAsync(Scenario scenario) => _scenarioRepository.UpdateAsync(scenario);
        public Task DeleteScenarioAsync(int id) => _scenarioRepository.DeleteAsync(id);

        // Kategori CRUD
        public Task<ScenarioCategory> GetCategoryByIdAsync(int id) => _categoryRepository.GetByIdAsync(id);
        public Task<IEnumerable<ScenarioCategory>> GetAllCategoriesAsync() => _categoryRepository.GetAllAsync();
        public async Task<ScenarioCategory> CreateCategoryAsync(ScenarioCategory category)
        {
            await _categoryRepository.AddAsync(category);
            return category;
        }
        public Task UpdateCategoryAsync(ScenarioCategory category) => _categoryRepository.UpdateAsync(category);
        public Task DeleteCategoryAsync(int id) => _categoryRepository.DeleteAsync(id);

        // Kişiselleştirilmiş Senaryo Üretme
        public async Task<Scenario> GeneratePersonalizedScenarioAsync(StudentProfile profile, string strategyType)
        {
            IScenarioGenerationStrategy selectedStrategy = null;

            // Strateji tipine göre doğru stratejiyi bul
            // Bu kısım daha dinamik hale getirilebilir (örn: strateji adları ile eşleştirme)
            if (strategyType.Equals("AI", StringComparison.OrdinalIgnoreCase) && _generationStrategies.OfType<AIModelScenarioStrategy>().Any())
            {
                selectedStrategy = _generationStrategies.OfType<AIModelScenarioStrategy>().First();
            }
            else if (strategyType.Equals("RuleBased", StringComparison.OrdinalIgnoreCase) && _generationStrategies.OfType<RuleBasedScenarioStrategy>().Any())
            {
                selectedStrategy = _generationStrategies.OfType<RuleBasedScenarioStrategy>().First();
            }
            else // Varsayılan veya bulunamazsa
            {
                selectedStrategy = _generationStrategies.OfType<RuleBasedScenarioStrategy>().FirstOrDefault() ??
                                   _generationStrategies.FirstOrDefault(); // Herhangi bir varsayılan
            }

            if (selectedStrategy == null)
            {
                throw new InvalidOperationException($"Strategy type '{strategyType}' not found or configured.");
            }

            var scenario = await selectedStrategy.GenerateScenarioAsync(profile);
            await _scenarioRepository.AddAsync(scenario); // Üretilen senaryoyu kaydet
            return scenario;
        }
    }
}