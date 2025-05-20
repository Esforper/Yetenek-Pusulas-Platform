using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.ValueObjects;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Strategies
{
    public interface IScenarioGenerationStrategy
    {
        Task<Scenario> GenerateScenarioAsync(StudentProfile profile);
    }
}