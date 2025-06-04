// Core/AIStrategies/GeminiAnalysisStrategy.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Core.Interfaces.Entities;

namespace YetenekPusulasi.Core.AIStrategies
{
    public class GeminiAnalysisStrategy : IAnalysisStrategy
    {
        public bool CanHandle(string modelIdentifier)
        {
            return modelIdentifier.StartsWith("Google-Gemini", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<AnalysisResult> PerformAnalysisAsync(StudentAnswer studentAnswer, IScenario scenario, IAIModelAdapter aiAdapter)
        {
            var result = new AnalysisResult
            {
                // StudentAnswerId = studentAnswer.Id,
                AiModelUsed = aiAdapter.ModelIdentifier
            };

            var systemPrompt = scenario.GetSystemPrompt();
            var userPrompt = studentAnswer.AnswerText;

            var aiRequest = new AIAdapterRequest { SystemPrompt = systemPrompt, UserPrompt = userPrompt };
            var aiResponse = await aiAdapter.GetCompletionAsync(aiRequest);
            result.RawAiResponse = aiResponse.Content ?? aiResponse.ErrorMessage;

            if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Content))
            {
                result.Summary = $"Gemini Değerlendirmesi: {aiResponse.Content.Substring(0, Math.Min(100, aiResponse.Content.Length))}...";
                result.OverallScore = 0.78; // Mock skor
                result.DetectedSkills.Add("Karar Verme (Mock)");
            }
            else
            {
                result.ErrorMessage = aiResponse.ErrorMessage ?? "Gemini analiz hatası.";
                result.Summary = "Analiz başarısız (Gemini).";
            }
            return result;
        }
    }
}