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
            var scenarioText = $"Kural tabanlı: '{profile.TargetSkillPreference}' yeteneği, zorluk {profile.PreferredDifficulty}.";
            var scenario = new Scenario
            {
                Text = scenarioText,
                TargetSkill = profile.TargetSkillPreference,
                DifficultyLevel = profile.PreferredDifficulty,
                ScenarioCategoryId = 1 // Varsayılan kategori
            };
            return Task.FromResult(scenario);
        }
    }
}