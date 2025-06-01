// Infrastructure/Data/Repositories/UserRepository.cs (YENİ)
using Microsoft.EntityFrameworkCore;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Repositories;
using System.Threading.Tasks;

namespace YetenekPusulasi.Infrastructure.Data.Repositories
{
    public class UserRepository : EfRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        // Örnek özel metot implementasyonu:
        // public async Task<ApplicationUser> GetByEmailAsync(string email)
        // {
        //     return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        // }

        // EfRepository<ApplicationUser>'dan gelen GetByIdAsync(string id) zaten iş görecektir.
    }
}