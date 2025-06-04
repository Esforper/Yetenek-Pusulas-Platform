// Core/Entities/Classroom.cs
using System.Collections.Generic; // ICollection için
using YetenekPusulasi.Data;       // ApplicationUser için
// Scenario için using eklenmeli, eğer farklı namespace'deyse

namespace YetenekPusulasi.Core.Entities
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } // Nullable olabilir
        public string ParticipationCode { get; set; }

        public string TeacherId { get; set; }
        public virtual ApplicationUser Teacher { get; set; }

        public virtual ICollection<StudentClassroom> StudentClassrooms { get; set; }
        public virtual ICollection<Scenario> Scenarios { get; set; } // <<< YENİ EKLEME: Bu sınıfa ait senaryolar

        public Classroom()
        {
            StudentClassrooms = new HashSet<StudentClassroom>();
            Scenarios = new HashSet<Scenario>(); // <<< YENİ EKLEME: Koleksiyonu başlat
        }
    }
}