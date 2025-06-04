// Core/Interfaces/Strategies/IInitialPromptGenerationStrategy.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Entities; // ScenarioType
using YetenekPusulasi.Core.Interfaces.AI; // IAIModelAdapter (AI stratejisi için)

namespace YetenekPusulasi.Core.Interfaces.Strategies
{
    public class InitialPromptContext // Stratejiye gönderilecek bilgiler
    {
        public string ScenarioTitle { get; set; }
        public string ScenarioDescription { get; set; }
        public ScenarioType ScenarioType { get; set; }
        public string? TeacherProvidedPrompt { get; set; } // Öğretmenin girdiği opsiyonel prompt
    }

    public interface IInitialPromptGenerationStrategy
    {
        // Dönen string, öğrencinin göreceği başlangıç prompt'u olacak.
        // bool wasAIGenerated, AI tarafından üretilip üretilmediğini dönecek.
        Task<(string generatedPrompt, bool wasAIGenerated)> GenerateInitialPromptAsync(InitialPromptContext context);
    }
}