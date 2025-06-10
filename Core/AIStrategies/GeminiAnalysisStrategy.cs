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


            string systemPrompt = $"{scenario.GetSystemPrompt()} " +
        "Öğrencinin aşağıdaki cevabını değerlendir. " +
        "Değerlendirmeni markdown formatında, başlıklar, listeler ve vurgular kullanarak yapılandırılmış bir özet olarak sun. " +
        "Özellikle güçlü yanlarını, geliştirilebilecek alanları ve genel bir değerlendirmeyi belirt.";
            var userPrompt = studentAnswer.AnswerText;

            var aiRequest = new AIAdapterRequest { SystemPrompt = systemPrompt, UserPrompt = $"Öğrenci Cevabı:\n\n{studentAnswer.AnswerText}", };
            _logger.LogInformation("Performing analysis with Gemini Mock strategy for answer ID: {AnswerId}", studentAnswer.Id);

            var aiResponse = await aiAdapter.GetCompletionAsync(aiRequest);
            result.RawAiResponse = aiResponse.Content ?? aiResponse.ErrorMessage;

            if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Content))
            {
                _logger.LogInformation("Gemini Mock analysis successful for answer ID: {AnswerId}. Mock Content: {Content}", studentAnswer.Id, aiResponse.Content);
                result.Summary = $"Gemini Değerlendirmesi: {aiResponse.Content.Substring(0, Math.Min(100, aiResponse.Content.Length))}...";
                result.OverallScore = ParseScoreFromResponse(aiResponse.Content); // Helper metot
                result.DetectedSkills = ParseSkillsFromResponse(aiResponse.Content); // Helper metot
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
        

                // Örnek Helper Metotlar (AI'dan gelen cevaba göre düzenlenmeli)
        private double? ParseScoreFromResponse(string? responseContent)
        {
            // Bu metot, AI'dan gelen metinden (belki markdown içindeki bir etiketten) skoru çıkarmalı.
            // Örnek: "Genel Değerlendirme Puanı: **%85**" gibi bir ifadeyi parse edebilir.
            // Şimdilik mock:
            if (string.IsNullOrWhiteSpace(responseContent)) return null;
            if (responseContent.ToLower().Contains("çok başarılı") || responseContent.ToLower().Contains("mükemmel")) return 0.9;
            return 0.7;
        }

        private List<string> ParseSkillsFromResponse(string? responseContent)
        {
            // Bu metot, AI'dan gelen metinden (belki markdown listelerinden) yetenekleri çıkarmalı.
            var skills = new List<string>();
            if (string.IsNullOrWhiteSpace(responseContent)) return skills;
            // Örnek: "- Analitik Düşünme\n- Yaratıcılık" gibi bir listeyi parse edebilir.
            if (responseContent.Contains("Analitik Düşünme")) skills.Add("Analitik Düşünme");
            if (responseContent.Contains("Problem Çözme")) skills.Add("Problem Çözme");
            return skills;
        }
    }
}