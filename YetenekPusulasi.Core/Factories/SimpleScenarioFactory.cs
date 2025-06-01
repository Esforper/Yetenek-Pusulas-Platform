// Core/Factories/SimpleScenarioFactory.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Factories;
namespace YetenekPusulasi.Core.Factories
{
    public class SimpleScenarioFactory : IScenarioFactory
    {
        public Scenario CreateScenario(string title, string content, ScenarioDifficulty difficulty, string targetSkill)
        {
            // Basit bir senaryo nesnesi oluşturma
            return new Scenario
            {
                Title = title,
                TextContent = content,
                Difficulty = difficulty,
                TargetSkill = targetSkill
            };
        }
    }
}