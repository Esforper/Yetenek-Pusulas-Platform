// Core/Strategies/ManualInitialPromptStrategy.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Strategies;

namespace YetenekPusulasi.Core.Strategies
{
    public class ManualInitialPromptStrategy : IInitialPromptGenerationStrategy
    {
        public Task<(string generatedPrompt, bool wasAIGenerated)> GenerateInitialPromptAsync(InitialPromptContext context)
        {
            // Öğretmenin girdiği prompt'u doğrudan kullan veya varsayılan bir metin döndür.
            string prompt = !string.IsNullOrWhiteSpace(context.TeacherProvidedPrompt)
                ? context.TeacherProvidedPrompt
                : $"'{context.ScenarioTitle}' başlıklı senaryoya hoş geldiniz. Lütfen aşağıdaki açıklamayı dikkatlice okuyun ve ardından düşüncelerinizi belirtin."; // Varsayılan

            return Task.FromResult((prompt, false)); // AI üretmedi
        }
    }
}