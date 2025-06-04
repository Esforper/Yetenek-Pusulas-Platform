// Models/ClassroomViewModels/ClassroomDetailsViewModel.cs
using System.Collections.Generic; // List ve IEnumerable için
using YetenekPusulasi.Core.Entities; // Classroom için
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için
using YetenekPusulasi.Data; // ApplicationUser için

namespace YetenekPusulasi.WebApp.Models.ClassroomViewModels
{
    public class ClassroomDetailsViewModel
    {
        public Classroom Classroom { get; set; }
        public List<ApplicationUser> Students { get; set; } // Bu List<ApplicationUser> olarak kalabilir
        public IList<IScenario> Scenarios { get; set; } // <<< IList<IScenario> veya IEnumerable<IScenario> olarak değiştirildi

        public ClassroomDetailsViewModel()
        {
            Students = new List<ApplicationUser>();
            Scenarios = new List<IScenario>(); // Boş liste ile başlatmak iyi bir pratik
        }
    }
}