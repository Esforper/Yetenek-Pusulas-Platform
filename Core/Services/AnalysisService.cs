// Core/Services/AnalysisService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Core.Interfaces.Entities;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Areas.Identity.Data; // DbContext için

namespace YetenekPusulasi.Core.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IEnumerable<IAIModelAdapter> _aiAdapters;
        private readonly IEnumerable<IAnalysisStrategy> _analysisStrategies;
        private readonly ApplicationDbContext _context; // AnalysisResult'ı kaydetmek için
        private readonly ILogger<AnalysisService> _logger;

        public AnalysisService(
            IEnumerable<IAIModelAdapter> aiAdapters,
            IEnumerable<IAnalysisStrategy> analysisStrategies,
            ApplicationDbContext context,
            ILogger<AnalysisService> logger)
        {
            _aiAdapters = aiAdapters;
            _analysisStrategies = analysisStrategies;
            _context = context;
            _logger = logger;
        }

        public async Task<AnalysisResult> AnalyzeStudentAnswerAsync(StudentAnswer studentAnswer, IScenario scenario, string? preferredAiModelIdentifier = null)
        {
            IAIModelAdapter? selectedAdapter = null;
            IAnalysisStrategy? selectedStrategy = null;

            if (!string.IsNullOrEmpty(preferredAiModelIdentifier))
            {
                selectedAdapter = _aiAdapters.FirstOrDefault(a =>
                    a.ModelIdentifier.Equals(preferredAiModelIdentifier, StringComparison.OrdinalIgnoreCase));
                if (selectedAdapter != null)
                {
                    selectedStrategy = _analysisStrategies.FirstOrDefault(s => s.CanHandle(selectedAdapter.ModelIdentifier));
                }
            }

            // Eğer tercih edilen model bulunamazsa veya belirtilmemişse, varsayılan bir strateji/adapter seç
            if (selectedAdapter == null || selectedStrategy == null)
            {
                // Basit bir varsayılan seçim mantığı (örn: ilk uygun olanı seç)
                // Veya konfigürasyondan varsayılan bir model oku.
                selectedAdapter = _aiAdapters.FirstOrDefault(a => a.ModelIdentifier.StartsWith("OpenAI")); // OpenAI'yi varsayalım
                if (selectedAdapter != null)
                {
                    selectedStrategy = _analysisStrategies.FirstOrDefault(s => s.CanHandle(selectedAdapter.ModelIdentifier));
                }
            }

            if (selectedAdapter == null || selectedStrategy == null)
            {
                _logger.LogError("Uygun AI Adapter veya Analysis Strategy bulunamadı. Tercih edilen: {PreferredModel}", preferredAiModelIdentifier);
                var errorResult = new AnalysisResult { /*StudentAnswerId = studentAnswer.Id,*/ ErrorMessage = "Analiz için uygun AI modeli yapılandırılamadı.", AiModelUsed = "N/A" };
                // await SaveAnalysisResultAsync(errorResult); // Hatalı sonucu da kaydedebiliriz.
                return errorResult;
            }

            _logger.LogInformation("'{ModelIdentifier}' AI modeli ile '{ScenarioTitle}' senaryosunun '{AnswerId}' ID'li cevabı analiz ediliyor.",
                selectedAdapter.ModelIdentifier, scenario.Title, studentAnswer.Id);

            try
            {
                var analysisResult = await selectedStrategy.PerformAnalysisAsync(studentAnswer, scenario, selectedAdapter);
                await SaveAnalysisResultAsync(analysisResult);
                return analysisResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI analizi sırasında hata oluştu. Model: {Model}, Cevap ID: {AnswerId}",
                    selectedAdapter.ModelIdentifier, studentAnswer.Id);
                var errorResult = new AnalysisResult { /*StudentAnswerId = studentAnswer.Id,*/ ErrorMessage = $"Analiz hatası: {ex.Message}", AiModelUsed = selectedAdapter.ModelIdentifier };
                await SaveAnalysisResultAsync(errorResult);
                return errorResult;
            }
        }

        private async Task SaveAnalysisResultAsync(AnalysisResult result)
        {
            _context.AnalysisResults.Add(result);
            await _context.SaveChangesAsync();
        }
    }
}