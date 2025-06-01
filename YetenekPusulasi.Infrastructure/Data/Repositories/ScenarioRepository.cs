// Infrastructure/Data/Repositories/ScenarioRepository.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
namespace YetenekPusulasi.Infrastructure.Data.Repositories
{
    public class ScenarioRepository : EfRepository<Scenario>, IScenarioRepository
    {
        public ScenarioRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        // IScenarioRepository'ye özel metot implementasyonları buraya gelebilir
    }
}