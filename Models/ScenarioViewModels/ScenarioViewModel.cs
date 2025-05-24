namespace YetenekPusulasi.Models.ScenarioViewModels
{
    public class ScenarioViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string TargetSkill { get; set; }
        public int DifficultyLevel { get; set; }
        public int ScenarioCategoryId { get; set; }
        public string ScenarioCategoryName { get; set; }
    }
}