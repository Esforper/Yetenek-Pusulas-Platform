// Core/Entities/StudentAnswer.cs (YENİ DOSYA)
using System;
using YetenekPusulasi.Data; // ApplicationUser
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario

namespace YetenekPusulasi.Core.Entities
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public DateTime SubmissionDate { get; set; }

        public int ScenarioId { get; set; }
        public virtual IScenario Scenario { get; set; } // Artık IScenario kullanıyoruz

        public string StudentId { get; set; }
        public virtual ApplicationUser Student { get; set; }

        // Bu cevaba ait analiz sonucu (bire bir ilişki)
        public virtual AnalysisResult? AnalysisResult { get; set; }


        public StudentAnswer()
        {
            SubmissionDate = DateTime.UtcNow;
        }
    }
}