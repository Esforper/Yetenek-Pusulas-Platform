// Core/Entities/Scenarios/DecisionMakingScenario.cs
namespace YetenekPusulasi.Core.Entities.Scenarios
{
    public class DecisionMakingScenario : Scenario
    {
        public List<string> OptionsProvided { get; set; } // Karar için sunulan seçenekler (opsiyonel)

        public DecisionMakingScenario() : base(ScenarioType.DecisionMaking)
        {
            OptionsProvided = new List<string>();
        }

        public override string GetSystemPrompt()
        {
            string specificInstruction = "Öğrencinin verdiği kararın gerekçelerini, olası sonuçları ne kadar dikkate aldığını ve karar verme sürecindeki mantıksal adımlarını analiz et. Kararın arkasındaki değerleri ve öncelikleri belirlemeye çalış.";
            if (OptionsProvided.Any())
            {
                specificInstruction += $" Sunulan seçenekler şunlardı: {string.Join(", ", OptionsProvided)}.";
            }
            return FormatBasePrompt(specificInstruction);
        }
    }
}