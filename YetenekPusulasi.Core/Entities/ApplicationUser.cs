// Core/Entities/ApplicationUser.cs (Güncelleme)
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // ICollection için

namespace YetenekPusulasi.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Öğretmen olarak verdiği sınıflar
        public virtual ICollection<Classroom> TaughtClasses { get; set; } = new HashSet<Classroom>();

        // Öğrenci olarak katıldığı sınıf-öğrenci kayıtları
        public virtual ICollection<StudentClassroom> EnrolledStudentClassrooms { get; set; } = new HashSet<StudentClassroom>();
    }
}