// Core/Entities/Classroom.cs
namespace YetenekPusulasi.Core.Entities
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParticipationCode { get; set; }
        public string TeacherId { get; set; }
        public virtual ApplicationUser Teacher { get; set; }
        public virtual ICollection<StudentClassroom> StudentClassrooms { get; set; } = new HashSet<StudentClassroom>();
    }
}