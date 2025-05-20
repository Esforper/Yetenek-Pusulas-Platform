using YetenekPusulasi.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IScenarioRepository
    {
        Task<Scenario> GetByIdAsync(int id);
        Task<IEnumerable<Scenario>> GetAllAsync();
        Task AddAsync(Scenario scenario);
        // UpdateAsync ve DeleteAsync da burada olurdu, öz olması için çıkarıldı.
    }
}