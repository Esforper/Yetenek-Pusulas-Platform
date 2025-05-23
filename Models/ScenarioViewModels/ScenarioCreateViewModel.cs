using System.ComponentModel.DataAnnotations;
namespace YetenekPusulasi.Models.ScenarioViewModels
{
    public class ScenarioCreateViewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string TargetSkill { get; set; }
        [Range(1, 5)]
        public int DifficultyLevel { get; set; }
        [Required]
        public int ScenarioCategoryId { get; set; }
    }
}