// Core/Interfaces/AI/IAnalysisStrategy.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities; // AnalysisResult, StudentAnswer için
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için

namespace YetenekPusulasi.Core.Interfaces.AI
{
    public interface IAnalysisStrategy
    {
        // Belirli bir AI modeliyle analiz yapma stratejisi
        Task<AnalysisResult> PerformAnalysisAsync(StudentAnswer studentAnswer, IScenario scenario, IAIModelAdapter aiAdapter);
        bool CanHandle(string modelIdentifier); // Bu stratejinin hangi AI modelini (adapter'ını) kullandığını belirtir
    }
}