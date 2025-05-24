using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.ValueObjects;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Strategies
{
    public class AIModelScenarioStrategy : IScenarioGenerationStrategy
    {
        // private readonly IExternalAIService _aiService; // Gerçek bir AI servisi enjekte edilebilir
        // public AIModelScenarioStrategy(IExternalAIService aiService) { _aiService = aiService; }

        public Task<Scenario> GenerateScenarioAsync(StudentProfile profile)
        {
            // string aiPrompt = $"...";
            // string generatedText = await _aiService.GenerateTextAsync(aiPrompt);
            string generatedText = $"AI Modeli: Kişiselleştirilmiş senaryo '{profile.TargetSkillPreference}', zorluk {profile.PreferredDifficulty}.";
            var scenario = new Scenario
            {
                Text = generatedText,
                TargetSkill = profile.TargetSkillPreference,
                DifficultyLevel = profile.PreferredDifficulty,
                ScenarioCategoryId = 2 // AI için farklı kategori
            };
            return Task.FromResult(scenario);
        }
    }
}