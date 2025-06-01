// Infrastructure/Data/Repositories/EfRepository.cs (Generic Repository Implementasyonu)
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace YetenekPusulasi.Infrastructure.Data.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public EfRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public virtual Task<T> GetByIdAsync(int id) => _dbContext.Set<T>().FindAsync(id).AsTask();
        public virtual Task<T> GetByIdAsync(string id) => _dbContext.Set<T>().FindAsync(id).AsTask();
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbContext.Set<T>().ToListAsync();
        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
        public virtual async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public virtual async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}