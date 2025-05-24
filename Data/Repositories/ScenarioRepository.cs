// Data/Repositories/ScenarioRepository.cs
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YetenekPusulasi.Data.Repositories
{
    public class ScenarioRepository : IScenarioRepository
    {
        private readonly ApplicationDbContext _context;

        public ScenarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Scenario> GetByIdAsync(int id) =>
            await _context.Scenarios
                          .Include(s => s.ScenarioCategory)
                          .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Scenario>> GetAllAsync() =>
            await _context.Scenarios
                          .Include(s => s.ScenarioCategory)
                          .ToListAsync();

        public async Task<IEnumerable<Scenario>> GetByCategoryIdAsync(int categoryId) =>
            await _context.Scenarios
                          .Where(s => s.ScenarioCategoryId == categoryId)
                          .Include(s => s.ScenarioCategory)
                          .ToListAsync();

        public async Task AddAsync(Scenario scenario)
        {
            await _context.Scenarios.AddAsync(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Scenario scenario)
        {
            _context.Entry(scenario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario != null)
            {
                _context.Scenarios.Remove(scenario);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) =>
            await _context.Scenarios.AnyAsync(e => e.Id == id);
    }
}