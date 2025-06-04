// Core/Entities/Scenarios/DecisionMakingScenario.cs
namespace YetenekPusulasi.Core.Entities.Scenarios
{
    public class AnalyticalThinkingScenario : Scenario
    {
        public List<string> OptionsProvided { get; set; } // Karar için sunulan seçenekler (opsiyonel)

        public AnalyticalThinkingScenario() : base(ScenarioType.AnalyticalThinking)
        {
            OptionsProvided = new List<string>();
        }

        public override string GetSystemPrompt()
        {
            string specificInstruction = "Öğrencinin analitik düşünme becerisini değerlendirmek için aşağıdaki senaryoya vereceği cevabı analiz edeceksin.";
            if (OptionsProvided.Any())
            {
                specificInstruction += $" Sunulan seçenekler şunlardı: {string.Join(", ", OptionsProvided)}.";
            }
            return FormatBasePrompt(specificInstruction);
        }
    }
}