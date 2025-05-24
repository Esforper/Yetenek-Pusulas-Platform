namespace YetenekPusulasi.Core.Entities
{
    public class ScenarioCategory
    {
       public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public ScenarioCategory()
        {
            Scenarios = new HashSet<Scenario>();
        }
    }
}