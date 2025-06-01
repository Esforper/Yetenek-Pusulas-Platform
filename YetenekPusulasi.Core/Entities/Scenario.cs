// Core/Entities/Scenario.cs
namespace YetenekPusulasi.Core.Entities
{
    public enum ScenarioDifficulty { Easy = 1, Medium, Hard }
    public class Scenario
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TextContent { get; set; }
        public ScenarioDifficulty Difficulty { get; set; }
        public string TargetSkill { get; set; }
        // public int CategoryId { get; set; }
        // public ScenarioCategory Category { get; set; }
    }
}
