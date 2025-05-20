using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Strategies;
using YetenekPusulasi.Core.ValueObjects;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Strategies
{
    public class RuleBasedScenarioStrategy : IScenarioGenerationStrategy
    {
        public Task<Scenario> GenerateScenarioAsync(StudentProfile profile)
        {
            // Basit kural: Öğrencinin tercih ettiği yetenek ve zorluğa göre bir senaryo oluştur.
            var scenarioText = $"Bu, '{profile.TargetSkillPreference}' yeteneğine odaklanan ve zorluğu {profile.PreferredDifficulty} olan kural tabanlı bir senaryodur.";
            var scenario = new Scenario
            {
                Text = scenarioText,
                TargetSkill = profile.TargetSkillPreference,
                DifficultyLevel = profile.PreferredDifficulty,
                CategoryId = 1 // Varsayılan bir kategori ID'si
            };
            return Task.FromResult(scenario);
        }
    }
}