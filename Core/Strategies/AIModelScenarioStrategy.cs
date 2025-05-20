using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.ValueObjects;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Strategies
{
    public class AIModelScenarioStrategy : IScenarioGenerationStrategy
    {
        // private readonly IAIThirdPartyService _aiService; // Gerçekte böyle bir servis enjekte edilirdi.

        public AIModelScenarioStrategy(/*IAIThirdPartyService aiService*/)
        {
            // _aiService = aiService;
        }

        public Task<Scenario> GenerateScenarioAsync(StudentProfile profile)
        {
            // AI servisinden senaryo talep etme simülasyonu
            // string aiGeneratedText = await _aiService.RequestScenarioText(profile);
            string aiGeneratedText = $"AI tarafından üretilmiş, '{profile.TargetSkillPreference}' yeteneği için kişiselleştirilmiş senaryo (zorluk {profile.PreferredDifficulty}).";

            var scenario = new Scenario
            {
                Text = aiGeneratedText,
                TargetSkill = profile.TargetSkillPreference,
                DifficultyLevel = profile.PreferredDifficulty,
                CategoryId = 2 // AI senaryoları için farklı bir kategori
            };
            return Task.FromResult(scenario);
        }
    }
}