namespace YetenekPusulasi.Core.Entities
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string TargetSkill { get; set; }
        public int DifficultyLevel { get; set; } // 1-5

        public int ScenarioCategoryId { get; set; }
        public virtual ScenarioCategory ScenarioCategory { get; set; }

        // Kimin oluşturduğu bilgisi eklenebilir (Identity User Id)
        // public string CreatedByUserId { get; set; }
        // public virtual ApplicationUser CreatedByUser { get; set; } // Eğer ApplicationUser'ı özelleştirdiyseniz
    }
}