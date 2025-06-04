// Core/Factories/ScenarioFactory.cs (MEVCUT DOSYAYI GÜNCELLE)
using System;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Entities.Scenarios; // Somut senaryo sınıfları için
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için

namespace YetenekPusulasi.Core.Factories
{
    public class ScenarioFactory
    {
        public ScenarioFactory() { }

        // Artık IScenario döndürecek
        public IScenario Create(string title, string description, ScenarioType type, string teacherId, int classroomId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Senaryo başlığı boş olamaz.", nameof(title));
            if (string.IsNullOrWhiteSpace(teacherId))
                throw new ArgumentException("Öğretmen ID'si boş olamaz.", nameof(teacherId));
            if (classroomId <= 0)
                throw new ArgumentException("Geçerli bir Sınıf ID'si belirtilmelidir.", nameof(classroomId));

            IScenario scenario; // IScenario tipinde değişken

            switch (type)
            {
                case ScenarioType.ProblemSolving:
                    scenario = new ProblemSolvingScenario();
                    // ProblemSolvingScenario'ya özgü property'ler burada atanabilir
                    // ((ProblemSolvingScenario)scenario).ProblemContext = "Varsayılan bağlam";
                    break;
                case ScenarioType.DecisionMaking:
                    scenario = new DecisionMakingScenario();
                    // ((DecisionMakingScenario)scenario).OptionsProvided.Add("Seçenek A");
                    break;
                case ScenarioType.AnalyticalThinking:
                    scenario = new AnalyticalThinkingScenario(); // Bu sınıfı oluşturduğunuzu varsayıyorum
                    break;
                case ScenarioType.EmpathyDevelopment:
                    scenario = new EmpathyDevelopmentScenario(); // Bu sınıfı oluşturduğunuzu varsayıyorum
                    break;
                case ScenarioType.CreativeThinking:
                    scenario = new CreativeThinkingScenario(); // Bu sınıfı oluşturduğunuzu varsayıyorum
                    break;
                case ScenarioType.Undefined:
                default:
                    // Belki genel bir senaryo tipi veya hata yönetimi
                    // Şimdilik tanımsız bir tip için temel bir senaryo (eğer varsa) veya exception
                    // throw new ArgumentOutOfRangeException(nameof(type), "Desteklenmeyen senaryo türü.");
                    // Veya en basit haliyle:
                    scenario = new GenericScenario(type); // Eğer Scenario abstract değilse veya GenericScenario varsa
                    break;
            }

            // Ortak property'leri ata
            scenario.Title = title;
            scenario.Description = description;
            // scenario.Type zaten constructor'da veya switch içinde set ediliyor
            scenario.TeacherId = teacherId;
            scenario.ClassroomId = classroomId;

            return scenario;
        }
    }
}