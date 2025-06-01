// Core/Entities/StudentClassroom.cs (Çoğa Çok)
namespace YetenekPusulasi.Core.Entities
{
    public class StudentClassroom
    {
        public string StudentId { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public int ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; }
    }
}