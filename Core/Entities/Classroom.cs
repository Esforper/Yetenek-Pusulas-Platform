namespace YetenekPusulasi.Core.Entities
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParticipationCode { get; set; } // Benzersiz katılım kodu

        public string TeacherId { get; set; } // IdentityUser'ın Id'si
        public virtual ApplicationUser Teacher { get; set; } // ApplicationUser'a referans

        public virtual ICollection<StudentClassroom> StudentClassrooms { get; set; } // Öğrencilerle ilişki (Çoğa Çok)

        public Classroom()
        {
            StudentClassrooms = new HashSet<StudentClassroom>();
        }
    }
}