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
        private readonly ILogger<GeminiAnalysisStrategy> _logger;
        public GeminiAnalysisStrategy(ILogger<GeminiAnalysisStrategy> logger) // IAIModelAdapter'ı constructor'dan almıyor, PerformAnalysis'te alıyor.
        {
            _logger = logger;
        }


        public bool CanHandle(string modelIdentifier)
        {
            return modelIdentifier.Equals("Google-Gemini-Mock", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<AnalysisResult> PerformAnalysisAsync(StudentAnswer studentAnswer, IScenario scenario, IAIModelAdapter aiAdapter)
        {

             _logger.LogInformation("GeminiAnalysisStrategy: PerformAnalysisAsync called for StudentAnswer.Id: {StudentAnswerId}", studentAnswer.Id);
            var result = new AnalysisResult
            {
                StudentAnswerId = studentAnswer.Id, // <<< BU SATIRIN KESİNLİKLE DOĞRU studentAnswer.Id'Yİ ALDIĞINDAN EMİN OLUN
                AiModelUsed = aiAdapter.ModelIdentifier,
                // Diğer alanları null veya varsayılan olarak başlatın
                Summary = null,
                OverallScore = null,
                DetectedSkills = new List<string>(),
                RawAiResponse = null,
                ErrorMessage = null,
                AnalysisDate = DateTime.UtcNow // Bu zaten AnalysisResult constructor'ında olmalı
            };
            

            var systemPrompt = scenario.GetSystemPrompt();
            var userPrompt = studentAnswer.AnswerText;

            var aiRequest = new AIAdapterRequest { SystemPrompt = systemPrompt, UserPrompt = userPrompt };
            _logger.LogInformation("Performing analysis with Gemini Mock strategy for answer ID: {AnswerId}", studentAnswer.Id);

            var aiResponse = await aiAdapter.GetCompletionAsync(aiRequest);
            result.RawAiResponse = aiResponse.Content ?? aiResponse.ErrorMessage;

            if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Content))
            {
                _logger.LogInformation("Gemini Mock analysis successful for answer ID: {AnswerId}. Mock Content: {Content}", studentAnswer.Id, aiResponse.Content);
                result.Summary = $"Gemini Değerlendirmesi: {aiResponse.Content.Substring(0, Math.Min(100, aiResponse.Content.Length))}...";
                result.OverallScore = 0.78; // Mock skor
                result.DetectedSkills.Add("Karar Verme (Mock)");
            }
            else
            {
                _logger.LogError("Gemini Mock analysis failed for answer ID: {AnswerId}. Error: {Error}", studentAnswer.Id, aiResponse.ErrorMessage);
                result.ErrorMessage = aiResponse.ErrorMessage ?? "Gemini analiz hatası.";
                result.Summary = "Analiz başarısız (Gemini).";
            }

            if (/* AI çağrısı başarısız */ !aiResponse.IsSuccess)
            {
                result.ErrorMessage = aiResponse.ErrorMessage;
                _logger.LogWarning("GeminiAnalysisStrategy: AI call failed for StudentAnswer.Id: {StudentAnswerId}. Error: {Error}", studentAnswer.Id, result.ErrorMessage);
            }
            _logger.LogInformation("GeminiAnalysisStrategy: Returning AnalysisResult for StudentAnswer.Id: {StudentAnswerId} with AnalysisResult.StudentAnswerId: {AnalysisStudentAnswerId}",
                studentAnswer.Id, result.StudentAnswerId);
            return result;
        }
    }
}