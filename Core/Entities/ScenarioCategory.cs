namespace YetenekPusulasi.Core.Entities
{
    public class ScenarioCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public ICollection<Scenario> Scenarios { get; set; } // Navigation (detaydan kaçınıldı)
    }
}