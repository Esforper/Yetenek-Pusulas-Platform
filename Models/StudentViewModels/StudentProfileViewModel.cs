// Models/StudentViewModels/StudentProfileViewModel.cs
using System.Collections.Generic;
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.WebApp.Models.StudentViewModels
{
    public class StudentProfileViewModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        // FirstName ve LastName property'leri kaldırıldı
        public List<StudentAnswer> SubmittedAnswers { get; set; }

        public StudentProfileViewModel()
        {
            SubmittedAnswers = new List<StudentAnswer>();
        }
    }
}