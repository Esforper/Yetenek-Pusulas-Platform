// Core/Interfaces/Repositories/IClassroomRepository.cs
using YetenekPusulasi.Core.Entities;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Interfaces.Repositories
{
    public interface IClassroomRepository : IRepository<Classroom>
    {
        Task<Classroom> GetByParticipationCodeAsync(string code);
    }
}