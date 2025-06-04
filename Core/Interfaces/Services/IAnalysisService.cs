// Core/Interfaces/Services/IAnalysisService.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Entities;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IAnalysisService
    {
        // Hangi AI modelinin kullanılacağı bir parametre ile belirtilebilir
        Task<AnalysisResult> AnalyzeStudentAnswerAsync(StudentAnswer studentAnswer, IScenario scenario, string preferredAiModelIdentifier = null);
    }
}