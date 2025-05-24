using YetenekPusulasi.Data;
using YetenekPusulasi.Models;

namespace YetenekPusulasi.Core.Entities
{
    public class StudentClassroom
    {
        public string StudentId { get; set; } // IdentityUser'ın Id'si
        public virtual ApplicationUser Student { get; set; }

        public int ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; }

        public DateTime JoinedDate { get; set; }
    }
}