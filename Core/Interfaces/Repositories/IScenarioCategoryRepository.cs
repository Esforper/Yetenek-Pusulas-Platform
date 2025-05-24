using YetenekPusulasi.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IScenarioCategoryRepository
    {
        Task<ScenarioCategory> GetByIdAsync(int id);
        Task<IEnumerable<ScenarioCategory>> GetAllAsync();
        Task AddAsync(ScenarioCategory category);
        Task UpdateAsync(ScenarioCategory category);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}