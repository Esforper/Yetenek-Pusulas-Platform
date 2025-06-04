// Core/Entities/Scenario.cs (MEVCUT DOSYAYI GÜNCELLE)
using System;
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için
using YetenekPusulasi.Data;

namespace YetenekPusulasi.Core.Entities
{
    public abstract class Scenario : IScenario // Artık abstract ve IScenario'yu implemente ediyor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ScenarioType Type { get; set; } // Type artık constructor'da veya alt sınıfta set edilecek
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public string TeacherId { get; set; }
        public virtual ApplicationUser Teacher { get; set; }

        public int ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; }

        protected Scenario(ScenarioType type) // Constructor'a type eklendi
        {
            CreatedDate = DateTime.UtcNow;
            Type = type;
        }

        // Abstract metot, alt sınıflar tarafından implemente edilmek zorunda
        public abstract string GetSystemPrompt();

        // Ortak helper metotlar veya property'ler buraya eklenebilir
        protected string FormatBasePrompt(string specificInstruction)
        {
            // Tüm senaryolar için ortak bir başlık veya talimat olabilir
            return $"Sen bir eğitim asistanısın. Öğrencinin bilişsel yeteneklerini değerlendirmek için aşağıdaki senaryoya vereceği cevabı analiz edeceksin. {specificInstruction}";
        }
    }
}