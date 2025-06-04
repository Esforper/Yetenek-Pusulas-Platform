// Core/AIStrategies/OpenAIAnalysisStrategy.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Core.Interfaces.Entities;

namespace YetenekPusulasi.Core.AIStrategies // Yeni namespace
{
    public class OpenAIAnalysisStrategy : IAnalysisStrategy
    {
        public bool CanHandle(string modelIdentifier)
        {
            // Bu stratejinin hangi adapter ile çalıştığını belirtir
            return modelIdentifier.StartsWith("OpenAI", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<AnalysisResult> PerformAnalysisAsync(StudentAnswer studentAnswer, IScenario scenario, IAIModelAdapter aiAdapter)
        {
            var result = new AnalysisResult
            {
                // StudentAnswerId = studentAnswer.Id, // StudentAnswer entity'si eklendiğinde
                AiModelUsed = aiAdapter.ModelIdentifier,
            };

            var systemPrompt = scenario.GetSystemPrompt(); // Her senaryo tipi kendi prompt'unu verir
            var userPrompt = studentAnswer.AnswerText;

            var aiRequest = new AIAdapterRequest
            {
                SystemPrompt = systemPrompt,
                UserPrompt = userPrompt,
                // Diğer OpenAI'ye özel parametreler (MaxTokens, Temperature) burada veya adapter'da set edilebilir.
            };

            var aiResponse = await aiAdapter.GetCompletionAsync(aiRequest);
            result.RawAiResponse = aiResponse.Content ?? aiResponse.ErrorMessage;

            if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Content))
            {
                // Gelen cevabı parse etme ve AnalysisResult'ı doldurma mantığı
                // Bu kısım oldukça karmaşık olabilir ve AI modelinin cevabına göre değişir.
                // Basit bir örnek:
                result.Summary = $"OpenAI Analizi: {aiResponse.Content.Substring(0, Math.Min(100, aiResponse.Content.Length))}...";
                result.OverallScore = 0.85; // Sabit bir skor (mock)
                result.DetectedSkills.Add("Problem Çözme (Mock)");
                result.DetectedSkills.Add("Analitik Düşünme (Mock)");
            }
            else
            {
                result.ErrorMessage = aiResponse.ErrorMessage ?? "Bilinmeyen bir analiz hatası oluştu.";
                result.Summary = "Analiz başarısız oldu.";
            }
            return result;
        }
    }
}