// Core/Strategies/RuleBasedScenarioStrategy.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Factories; // Senaryo üretmek için
using YetenekPusulasi.Core.Interfaces.Strategies;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Strategies
{
    public class RuleBasedScenarioStrategy : IScenarioGenerationStrategy
    {
        private readonly IScenarioFactory _scenarioFactory;
        public RuleBasedScenarioStrategy(IScenarioFactory scenarioFactory)
        {
            _scenarioFactory = scenarioFactory;
        }
        public Task<Scenario> GenerateScenarioAsync(string targetSkill, ScenarioDifficulty difficulty)
        {
            string title = $"Kural Tabanlı: {targetSkill}";
            string content = $"Bu senaryo '{targetSkill}' yeteneğini '{difficulty}' zorluğunda ölçer.";
            var scenario = _scenarioFactory.CreateScenario(title, content, difficulty, targetSkill);
            return Task.FromResult(scenario);
        }
    }
}