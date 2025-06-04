// Core/Entities/Scenarios/DecisionMakingScenario.cs
namespace YetenekPusulasi.Core.Entities.Scenarios
{
    public class CreativeThinkingScenario : Scenario
    {
        public List<string> OptionsProvided { get; set; } // Karar için sunulan seçenekler (opsiyonel)

        public CreativeThinkingScenario() : base(ScenarioType.CreativeThinking)
        {
            OptionsProvided = new List<string>();
        }

        public override string GetSystemPrompt()
        {
            string specificInstruction = "Öğrencinin yaratıcılığını, yenilikçi düşünme becerisini ve problem çözme yeteneğini değerlendirmek için aşağıdaki senaryoya vereceği cevabı analiz edeceksin.";
            if (OptionsProvided.Any())
            {
                specificInstruction += $" Sunulan seçenekler şunlardı: {string.Join(", ", OptionsProvided)}.";
            }
            return FormatBasePrompt(specificInstruction);
        }
    }
}