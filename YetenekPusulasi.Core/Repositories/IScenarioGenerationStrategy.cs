// Core/Interfaces/Strategies/IScenarioGenerationStrategy.cs (STRATEGY)
using YetenekPusulasi.Core.Entities;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Interfaces.Strategies
{
    public interface IScenarioGenerationStrategy
    {
        // StudentProfile gibi bir parametre alabilir
        Task<Scenario> GenerateScenarioAsync(string targetSkill, ScenarioDifficulty difficulty);
    }
}