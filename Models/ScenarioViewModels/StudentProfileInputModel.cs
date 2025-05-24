// Models/ScenarioViewModels/StudentProfileInputModel.cs (API isteği için)
using System.ComponentModel.DataAnnotations;
namespace YetenekPusulasi.Models.ScenarioViewModels
{
    public class StudentProfileInputModel
    {
        [Required]
        public string TargetSkillPreference { get; set; }
        [Range(1, 5)]
        public int PreferredDifficulty { get; set; }
    }
}