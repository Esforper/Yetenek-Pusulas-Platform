// Core/Interfaces/Entities/IScenario.cs (YENİ KLASÖR VE DOSYA)
using System;
using YetenekPusulasi.Core.Entities; // ScenarioType enum için
using YetenekPusulasi.Data;         // ApplicationUser için

namespace YetenekPusulasi.Core.Interfaces.Entities
{
    public interface IScenario
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        ScenarioType Type { get; }
        DateTime CreatedDate { get; set; }
        DateTime? LastModifiedDate { get; set; }

        string TeacherId { get; set; }
        ApplicationUser Teacher { get; set; }

        int ClassroomId { get; set; }
        Classroom Classroom { get; set; }


        string? InitialPrompt { get; set; } // Öğrencinin göreceği başlangıç prompt'u
        bool WasInitialPromptAIGenerated { get; set; } // Bu prompt AI tarafından mı üretildi?

        // Her senaryo türünün kendine özgü bir sistem prompt'u döndürmesini sağlayacak metot
        string GetSystemPrompt();

        // Belki senaryonun öğrenciye nasıl sunulacağına dair bir metot
        // string GetDisplayHtmlForStudent();

        // İleride eklenebilecek diğer ortak metotlar veya property'ler
    }
}