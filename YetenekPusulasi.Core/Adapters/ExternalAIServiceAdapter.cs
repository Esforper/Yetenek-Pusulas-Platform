// Core/Adapters/ExternalAIServiceAdapter.cs (ADAPTER Deseni Örneği)
// Bu aynı zamanda bir IScenarioGenerationStrategy olabilir
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.Interfaces.Factories;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Adapters
{
    // Varsayımsal Dış AI Kütüphanesi
    namespace ThirdPartyAI
    {
        public class AdvancedAISdk
        {
            public Task<string> CreateComplexScenarioAsync(string skill, int diffLevel, string studentContext)
            {
                return Task.FromResult($"Advanced AI: Skill={skill}, Diff={diffLevel}, Context={studentContext} -> GENERATED TEXT");
            }
        }
    }

    public class ExternalAIServiceAdapter : IScenarioGenerationStrategy
    {
        private readonly ThirdPartyAI.AdvancedAISdk _externalAIService;
        private readonly IScenarioFactory _scenarioFactory;

        public ExternalAIServiceAdapter(ThirdPartyAI.AdvancedAISdk externalAIService, IScenarioFactory scenarioFactory)
        {
            _externalAIService = externalAIService;
            _scenarioFactory = scenarioFactory;
        }

        public async Task<Scenario> GenerateScenarioAsync(string targetSkill, ScenarioDifficulty difficulty)
        {
            // Adaptasyon: Bizim girdilerimizi dış servisin beklediği formata çevir
            string studentContextInfo = "Generic Student Profile"; // Örnek
            int diffLevel = (int)difficulty;

            string aiGeneratedText = await _externalAIService.CreateComplexScenarioAsync(targetSkill, diffLevel, studentContextInfo);

            // Adaptasyon: Dış servisin çıktısını kendi Scenario objemize çevir
            return _scenarioFactory.CreateScenario(
                $"AI ({targetSkill})",
                aiGeneratedText,
                difficulty,
                targetSkill);
        }
    }
}