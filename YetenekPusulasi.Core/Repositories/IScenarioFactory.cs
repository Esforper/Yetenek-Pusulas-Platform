// Core/Interfaces/Factories/IScenarioFactory.cs (FACTORY METHOD)
using YetenekPusulasi.Core.Entities;
namespace YetenekPusulasi.Core.Interfaces.Factories
{
    public interface IScenarioFactory // Bu, senaryo nesnesini üretir
    {
        Scenario CreateScenario(string title, string content, ScenarioDifficulty difficulty, string targetSkill);
    }
}