using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Data.Repositories
{
    public class ScenarioCategoryRepository : IScenarioCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ScenarioCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ScenarioCategory> GetByIdAsync(int id) =>
             await _context.ScenarioCategories.Include(sc => sc.Scenarios).FirstOrDefaultAsync(sc => sc.Id == id);

        public async Task<IEnumerable<ScenarioCategory>> GetAllAsync() =>
             await _context.ScenarioCategories.ToListAsync();

        public async Task AddAsync(ScenarioCategory category)
        {
            await _context.ScenarioCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ScenarioCategory category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.ScenarioCategories.FindAsync(id);
            if (category != null)
            {
                // Kategoriye bağlı senaryolar varsa ne yapılacağına karar verilmeli (OnDelete Restrict ayarlandı)
                _context.ScenarioCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) =>
            await _context.ScenarioCategories.AnyAsync(e => e.Id == id);
    }
}
