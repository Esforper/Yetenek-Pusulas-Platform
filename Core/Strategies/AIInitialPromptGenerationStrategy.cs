// Core/Strategies/AIInitialPromptGenerationStrategy.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Core.Interfaces.Strategies;

namespace YetenekPusulasi.Core.Strategies
{
    public class AIInitialPromptGenerationStrategy : IInitialPromptGenerationStrategy
    {
        private readonly IAIModelAdapter _aiAdapter; // Hangi AI modeliyle prompt üretileceği
        private readonly ILogger<AIInitialPromptGenerationStrategy> _logger;

        public AIInitialPromptGenerationStrategy(IAIModelAdapter aiAdapter, ILogger<AIInitialPromptGenerationStrategy> logger)
        {
            _aiAdapter = aiAdapter; // Bu strateji belirli bir AI Adapter ile çalışır
            _logger = logger;
        }

        public async Task<(string generatedPrompt, bool wasAIGenerated)> GenerateInitialPromptAsync(InitialPromptContext context)
        {
            // AI'a gönderilecek sistem prompt'u ve kullanıcı prompt'unu oluştur
            string systemMessageForPromptGeneration = $"Sen bir eğitim materyali yazarısın. Aşağıdaki bilgiler verilen bir senaryo için ilgi çekici ve düşündürücü bir başlangıç metni (ilk paragraf veya giriş sorusu) oluşturacaksın. Bu metin, öğrencinin senaryoya odaklanmasını sağlamalı. Senaryo Türü: {context.ScenarioType.ToString()}.";
            if (!string.IsNullOrWhiteSpace(context.TeacherProvidedPrompt))
            {
                systemMessageForPromptGeneration += $" Öğretmenin özel bir isteği var: '{context.TeacherProvidedPrompt}'. Bu isteği de dikkate alarak bir başlangıç metni hazırla.";
            }

            string userMessageForPromptGeneration = $"Senaryo Başlığı: {context.ScenarioTitle}\nSenaryo Açıklaması: {context.ScenarioDescription}\n\nLütfen bu senaryo için bir başlangıç metni oluştur.";

            var aiRequest = new AIAdapterRequest
            {
                SystemPrompt = systemMessageForPromptGeneration,
                UserPrompt = userMessageForPromptGeneration,
                MaxTokens = 100, // Başlangıç prompt'u için daha kısa olabilir
                Temperature = 0.5
            };

            _logger.LogInformation("AI'dan başlangıç prompt'u isteniyor. Model: {Model}, Senaryo: {Title}", _aiAdapter.ModelIdentifier, context.ScenarioTitle);

            var aiResponse = await _aiAdapter.GetCompletionAsync(aiRequest);

            if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Content))
            {
                _logger.LogInformation("AI'dan başlangıç prompt'u başarıyla alındı.");
                return (aiResponse.Content, true); // AI üretti
            }
            else
            {
                _logger.LogError("AI'dan başlangıç prompt'u alınırken hata oluştu: {Error}", aiResponse.ErrorMessage);
                // Hata durumunda varsayılan bir manuel prompt'a düşebiliriz
                string fallbackPrompt = $"'{context.ScenarioTitle}' senaryosuna hoş geldiniz. Lütfen aşağıdaki açıklamayı dikkatlice okuyun ve ardından düşüncelerinizi belirtin. (AI prompt üretilemedi)";
                return (fallbackPrompt, false); // AI üretemedi
            }
        }
    }
}