// Core/Interfaces/Repositories/IRepository.cs (Generic Repository Arayüzü)
using System.Collections.Generic;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id); // Guid veya string ID'ler için
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}