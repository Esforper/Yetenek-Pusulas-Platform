// Core/Interfaces/Services/IScenarioService.cs
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IScenarioService
    {
        Task<Scenario> GetScenarioByIdAsync(int id);
        Task<IEnumerable<Scenario>> GetAllScenariosAsync();
        Task<Scenario> CreateScenarioAsync(Scenario scenario); // Manuel oluşturma
        Task UpdateScenarioAsync(Scenario scenario);
        Task DeleteScenarioAsync(int id);

        Task<ScenarioCategory> GetCategoryByIdAsync(int id);
        Task<IEnumerable<ScenarioCategory>> GetAllCategoriesAsync();
        Task<ScenarioCategory> CreateCategoryAsync(ScenarioCategory category);
        Task UpdateCategoryAsync(ScenarioCategory category);
        Task DeleteCategoryAsync(int id);

        Task<Scenario> GeneratePersonalizedScenarioAsync(StudentProfile profile, string strategyType);
        // void SetScenarioGenerationStrategy(IScenarioGenerationStrategy strategy); // Eğer service içinde strateji set edilecekse
    }
}