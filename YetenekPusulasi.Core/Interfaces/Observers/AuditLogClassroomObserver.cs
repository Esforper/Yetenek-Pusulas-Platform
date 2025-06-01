// Core/Observers/AuditLogClassroomObserver.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Observers;
using System;
namespace YetenekPusulasi.Core.Observers
{
    public class AuditLogClassroomObserver : IClassroomObserver
    {
        public void OnClassroomCreated(Classroom classroom)
        {
            Console.WriteLine($"AUDIT LOG: Classroom '{classroom.Name}' (Code: {classroom.ParticipationCode}) created by Teacher ID: {classroom.TeacherId}");
        }
        public void OnStudentJoined(Classroom classroom, ApplicationUser student)
        {
            Console.WriteLine($"AUDIT LOG: Student '{student.Email}' joined classroom '{classroom.Name}'");
        }
    }
}