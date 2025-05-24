using System.ComponentModel.DataAnnotations;
namespace YetenekPusulasi.Models.ScenarioViewModels
{
    public class ScenarioCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}