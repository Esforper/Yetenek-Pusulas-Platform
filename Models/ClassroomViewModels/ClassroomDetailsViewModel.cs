// Models/ClassroomViewModels/ClassroomDetailsViewModel.cs (YENİ DOSYA)
using YetenekPusulasi.Data;

namespace YetenekPusulasi.WebApp.Models.ClassroomViewModels
{
    public class ClassroomDetailsViewModel
    {
        public Core.Entities.Classroom Classroom { get; set; }
        public List<ApplicationUser> Students { get; set; }
        public List<Core.Entities.Scenario> Scenarios { get; set; }
    }
}