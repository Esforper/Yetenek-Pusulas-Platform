// Controllers/ScenariosController.cs
using Microsoft.AspNetCore.Mvc;
using YetenekPusulasi.Core.Interfaces.Services; // IScenarioService
using YetenekPusulasi.Core.Entities; // Scenario, ScenarioCategory
using YetenekPusulasi.Core.ValueObjects; // StudentProfile
using YetenekPusulasi.Models.ScenarioViewModels; // ViewModels
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Select için
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için

namespace YetenekPusulasi.Controllers
{
    [Authorize] // Bu controller'a erişim için kimlik doğrulaması gereksin (opsiyonel, rol bazlı da olabilir)
    [Route("api/[controller]")]
    [ApiController]
    public class ScenariosController : ControllerBase
    {
        private readonly IScenarioService _scenarioService;

        public ScenariosController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        // GET: api/Scenarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScenarioViewModel>>> GetScenarios()
        {
            var scenarios = await _scenarioService.GetAllScenariosAsync();
            return Ok(scenarios.Select(s => new ScenarioViewModel // AutoMapper da kullanılabilir
            {
                Id = s.Id,
                Text = s.Text,
                TargetSkill = s.TargetSkill,
                DifficultyLevel = s.DifficultyLevel,
                ScenarioCategoryId = s.ScenarioCategoryId,
                ScenarioCategoryName = s.ScenarioCategory?.Name
            }));
        }

        // GET: api/Scenarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScenarioViewModel>> GetScenario(int id)
        {
            var scenario = await _scenarioService.GetScenarioByIdAsync(id);
            if (scenario == null) return NotFound();
            return Ok(new ScenarioViewModel
            {
                Id = scenario.Id,
                Text = scenario.Text,
                TargetSkill = scenario.TargetSkill,
                DifficultyLevel = scenario.DifficultyLevel,
                ScenarioCategoryId = scenario.ScenarioCategoryId,
                ScenarioCategoryName = scenario.ScenarioCategory?.Name
            });
        }

        // POST: api/Scenarios
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")] // Sadece Admin veya Teacher rollerindekiler oluşturabilsin
        public async Task<ActionResult<ScenarioViewModel>> PostScenario(ScenarioCreateViewModel createModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var scenario = new Scenario
            {
                Text = createModel.Text,
                TargetSkill = createModel.TargetSkill,
                DifficultyLevel = createModel.DifficultyLevel,
                ScenarioCategoryId = createModel.ScenarioCategoryId
                // CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Giriş yapmış kullanıcı ID'si
            };

            var createdScenario = await _scenarioService.CreateScenarioAsync(scenario);
            var viewModel = new ScenarioViewModel // Yanıt için ViewModel'a map et
            {
                Id = createdScenario.Id,
                Text = createdScenario.Text,
                TargetSkill = createdScenario.TargetSkill,
                DifficultyLevel = createdScenario.DifficultyLevel,
                ScenarioCategoryId = createdScenario.ScenarioCategoryId,
                ScenarioCategoryName = (await _scenarioService.GetCategoryByIdAsync(createdScenario.ScenarioCategoryId))?.Name
            };
            return CreatedAtAction(nameof(GetScenario), new { id = createdScenario.Id }, viewModel);
        }

        // PUT: api/Scenarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> PutScenario(int id, ScenarioCreateViewModel updateModel) // Update için de CreateViewModel kullanılabilir
        {
            var scenarioToUpdate = await _scenarioService.GetScenarioByIdAsync(id);
            if (scenarioToUpdate == null) return NotFound();

            // Güncelleme (AutoMapper ile daha temiz olabilir)
            scenarioToUpdate.Text = updateModel.Text;
            scenarioToUpdate.TargetSkill = updateModel.TargetSkill;
            scenarioToUpdate.DifficultyLevel = updateModel.DifficultyLevel;
            scenarioToUpdate.ScenarioCategoryId = updateModel.ScenarioCategoryId;

            await _scenarioService.UpdateScenarioAsync(scenarioToUpdate);
            return NoContent();
        }

        // DELETE: api/Scenarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteScenario(int id)
        {
            var scenario = await _scenarioService.GetScenarioByIdAsync(id);
            if (scenario == null) return NotFound();
            await _scenarioService.DeleteScenarioAsync(id);
            return NoContent();
        }

        // --- Kategoriler için Endpoints ---
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ScenarioCategoryViewModel>>> GetCategories()
        {
            var categories = await _scenarioService.GetAllCategoriesAsync();
            return Ok(categories.Select(c => new ScenarioCategoryViewModel { Id = c.Id, Name = c.Name, Description = c.Description }));
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin")] // Sadece Admin kategori oluşturabilsin
        public async Task<ActionResult<ScenarioCategoryViewModel>> PostCategory(ScenarioCategoryViewModel createModel)
        {
            var category = new ScenarioCategory { Name = createModel.Name, Description = createModel.Description };
            var createdCategory = await _scenarioService.CreateCategoryAsync(category);
            // ... yanıt oluşturma
            return CreatedAtAction(nameof(GetCategories), new { id = createdCategory.Id }, new ScenarioCategoryViewModel { Id = createdCategory.Id, Name = createdCategory.Name, Description = createdCategory.Description });
        }

        // --- Kişiselleştirilmiş Senaryo Üretme ---
        [HttpPost("generate-personalized")]
        // [Authorize(Roles = "Teacher")] // Öğretmenler tetikleyebilir
        public async Task<ActionResult<ScenarioViewModel>> GeneratePersonalizedScenario(
            [FromBody] StudentProfileInputModel profileInput,
            [FromQuery] string strategyType = "RuleBased") // "AI" veya "RuleBased"
        {
            if (profileInput == null) return BadRequest("Student profile input is required.");

            var studentProfile = new StudentProfile(profileInput.TargetSkillPreference, profileInput.PreferredDifficulty);
            var scenario = await _scenarioService.GeneratePersonalizedScenarioAsync(studentProfile, strategyType);

            if (scenario == null) return StatusCode(500, "Could not generate scenario.");

            var viewModel = new ScenarioViewModel
            {
                Id = scenario.Id, Text = scenario.Text, TargetSkill = scenario.TargetSkill,
                DifficultyLevel = scenario.DifficultyLevel, ScenarioCategoryId = scenario.ScenarioCategoryId,
                ScenarioCategoryName = (await _scenarioService.GetCategoryByIdAsync(scenario.ScenarioCategoryId))?.Name
            };
            return Ok(viewModel);
        }
    }
}