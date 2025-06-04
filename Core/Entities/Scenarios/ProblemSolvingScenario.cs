// Core/Entities/Scenarios/ProblemSolvingScenario.cs
namespace YetenekPusulasi.Core.Entities.Scenarios // Yeni namespace
{
    public class ProblemSolvingScenario : Scenario
    {
        public string ProblemContext { get; set; } // Probleme özgü ek bir alan olabilir
        public int ExpectedSolutionSteps { get; set; } // Örnek ek bir alan

        // Constructor, base class'ın constructor'ını çağırır
        public ProblemSolvingScenario() : base(ScenarioType.ProblemSolving)
        {
            ProblemContext = string.Empty; // Varsayılan değer
        }

        public override string GetSystemPrompt()
        {
            string specificInstruction = $"Öğrencinin verdiği cevaptaki problem çözme adımlarını, mantıksal tutarlılığını ve çözümün etkinliğini değerlendir. Özellikle şu adımlara odaklan: problemi anlama, olası çözümler üretme, en iyi çözümü seçme ve uygulama planı.";
            if (!string.IsNullOrEmpty(ProblemContext))
            {
                specificInstruction += $" Senaryo bağlamı: {ProblemContext}.";
            }
            return FormatBasePrompt(specificInstruction);
        }
    }
}