// Core/Entities/Scenario.cs
using System;
using System.Collections.Generic; // ICollection için
using YetenekPusulasi.Data; // ApplicationUser için
// using YetenekPusulasi.Core.Entities; // Classroom ve StudentAnswer için (aşağıda tanımlanacak)

namespace YetenekPusulasi.Core.Entities
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ScenarioType Type { get; set; } // Enum türümüz
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        // İlişkiler
        public string TeacherId { get; set; } // Senaryoyu oluşturan öğretmen
        public virtual ApplicationUser Teacher { get; set; }

        public int ClassroomId { get; set; } // Senaryonun atandığı sınıf
        public virtual Classroom Classroom { get; set; }

        // Öğrencilerin bu senaryoya verdiği cevaplar (ileride eklenecek)
        // public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }

        public Scenario()
        {
            CreatedDate = DateTime.UtcNow;
            // StudentAnswers = new HashSet<StudentAnswer>();
        }
    }
}