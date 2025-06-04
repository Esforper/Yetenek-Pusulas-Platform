// Core/Factories/ScenarioFactory.cs
using System;
using System.Threading.Tasks; // Task için eklendi
using Microsoft.Extensions.DependencyInjection; // IServiceProvider ve GetRequiredService için
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Entities.Scenarios; // Somut senaryo sınıfları için
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için
using YetenekPusulasi.Core.Interfaces.Strategies; // IInitialPromptGenerationStrategy için
using YetenekPusulasi.Core.Strategies; // Somut stratejiler için (ManualInitialPromptStrategy, AIInitialPromptGenerationStrategy)

namespace YetenekPusulasi.Core.Factories
{
    public class ScenarioFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScenarioFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // Metot artık async oldu çünkü prompt üretimi AI ile async olabilir
        // ve IScenario döndürecek.
        public async Task<IScenario> CreateAsync(
            string title,
            string description,
            ScenarioType type,
            string teacherId,
            int classroomId,
            string? teacherProvidedInitialPrompt, // Öğretmenin girdiği opsiyonel başlangıç prompt'u
            bool generateInitialPromptWithAI)     // Başlangıç prompt'u AI ile mi üretilecek flag'i
        {
            // Temel parametre kontrolleri
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Senaryo başlığı boş olamaz.", nameof(title));
            if (string.IsNullOrWhiteSpace(teacherId))
                throw new ArgumentException("Öğretmen ID'si boş olamaz.", nameof(teacherId));
            if (classroomId <= 0)
                throw new ArgumentException("Geçerli bir Sınıf ID'si belirtilmelidir.", nameof(classroomId));

            // 1. Başlangıç Prompt'unu Üretmek İçin Stratejiyi Belirle ve Kullan
            IInitialPromptGenerationStrategy promptStrategy;
            if (generateInitialPromptWithAI)
            {
                // AI stratejisini DI'dan al.
                // Bu stratejinin constructor'ında uygun IAIModelAdapter enjekte edilmiş olmalı.
                promptStrategy = _serviceProvider.GetRequiredService<AIInitialPromptGenerationStrategy>();
            }
            else
            {
                promptStrategy = _serviceProvider.GetRequiredService<ManualInitialPromptStrategy>();
            }

            var promptContext = new InitialPromptContext // Bu DTO'yu bir önceki mesajda tanımlamıştık
            {
                ScenarioTitle = title,
                ScenarioDescription = description,
                ScenarioType = type,
                TeacherProvidedPrompt = teacherProvidedInitialPrompt
            };

            // Stratejiyi kullanarak başlangıç prompt'unu ve nasıl üretildiğini al
            var (initialPromptContent, wasAIGenerated) = await promptStrategy.GenerateInitialPromptAsync(promptContext);

            // 2. Senaryo Türüne Göre Somut Senaryo Nesnesini Oluştur
            IScenario scenario; // IScenario tipinde değişken

            switch (type)
            {
                case ScenarioType.ProblemSolving:
                    scenario = new ProblemSolvingScenario();
                    // ProblemSolvingScenario'ya özgü ek property'ler burada atanabilir, örneğin:
                    // if (scenario is ProblemSolvingScenario psScenario)
                    // {
                    //     psScenario.ProblemContext = "Fabrikada üretim hattı durdu..."; // Örnek
                    // }
                    break;
                case ScenarioType.DecisionMaking:
                    scenario = new DecisionMakingScenario();
                    // if (scenario is DecisionMakingScenario dmScenario)
                    // {
                    //     dmScenario.OptionsProvided.Add("A seçeneği: Riskli ama yüksek kazançlı.");
                    //     dmScenario.OptionsProvided.Add("B seçeneği: Güvenli ama düşük kazançlı.");
                    // }
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
                    scenario = new GenericScenario(type); // Gelen 'type'ı GenericScenario'ya iletiyoruz
                    break;
            }

            // 3. Oluşturulan Senaryo Nesnesine Ortak ve Yeni Property'leri Ata
            scenario.Title = title;
            scenario.Description = description;
            // scenario.Type zaten her somut senaryonun kendi constructor'ında base class'a iletiliyor.
            scenario.TeacherId = teacherId;
            scenario.ClassroomId = classroomId;
            scenario.InitialPrompt = initialPromptContent; // Üretilen veya manuel girilen başlangıç prompt'u
            scenario.WasInitialPromptAIGenerated = wasAIGenerated; // AI tarafından mı üretildi bilgisi
            scenario.LastModifiedDate = DateTime.UtcNow; // Oluşturulurken son düzenleme tarihi de set edilebilir

            return scenario;
        }
    }
}