// Core/Entities/Scenarios/DecisionMakingScenario.cs
namespace YetenekPusulasi.Core.Entities.Scenarios
{
    public class EmpathyDevelopmentScenario : Scenario
    {
        public List<string> EmpathyContexts { get; set; } // Empati için bağlamlar (opsiyonel)

        public EmpathyDevelopmentScenario() : base(ScenarioType.EmpathyDevelopment)
        {
            EmpathyContexts = new List<string>();
        }

        public override string GetSystemPrompt()
        {
            string specificInstruction = "Öğrencinin empati becerisini değerlendirmek için aşağıdaki senaryoya vereceği cevabı analiz edeceksin.";
            if (EmpathyContexts.Any())
            {
                specificInstruction += $" Sunulan bağlamlar şunlardı: {string.Join(", ", EmpathyContexts)}.";
            }
            return FormatBasePrompt(specificInstruction);
        }
    }
}