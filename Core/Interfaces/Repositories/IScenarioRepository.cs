using YetenekPusulasi.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IScenarioRepository
    {
        Task<Scenario> GetByIdAsync(int id);
        Task<IEnumerable<Scenario>> GetAllAsync();
        Task<IEnumerable<Scenario>> GetByCategoryIdAsync(int categoryId);
        Task AddAsync(Scenario scenario);
        Task UpdateAsync(Scenario scenario);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}