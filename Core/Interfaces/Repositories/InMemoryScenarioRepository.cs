using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YetenekPusulasi.Infrastructure.Data.Repositories
{
    public class InMemoryScenarioRepository : IScenarioRepository
    {
        private readonly List<Scenario> _scenarios = new List<Scenario>();
        private int _nextId = 1;

        public Task<Scenario> GetByIdAsync(int id)
        {
            return Task.FromResult(_scenarios.FirstOrDefault(s => s.Id == id));
        }

        public Task<IEnumerable<Scenario>> GetAllAsync()
        {
            return Task.FromResult(_scenarios.AsEnumerable());
        }

        public Task AddAsync(Scenario scenario)
        {
            scenario.Id = _nextId++;
            _scenarios.Add(scenario);
            return Task.CompletedTask;
        }
    }
}