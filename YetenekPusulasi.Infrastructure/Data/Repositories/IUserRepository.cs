// Core/Interfaces/Repositories/IUserRepository.cs (YENİ)
using YetenekPusulasi.Core.Entities;

namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        // ApplicationUser'a özel sorgulama metotları buraya eklenebilir
        // Örneğin: Task<ApplicationUser> GetByEmailAsync(string email);
    }
}