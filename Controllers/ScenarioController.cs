// Controllers/ScenarioController.cs
using Microsoft.AspNetCore.Mvc;
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Core.ValueObjects;
using YetenekPusulasi.Core.Strategies; // Stratejileri direkt enjekte etmek için (ya da bir factory ile)
using System.Threading.Tasks;

namespace YetenekPusulasi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScenariosController : ControllerBase
    {
        private readonly ScenarioService _scenarioService;
        // Farklı stratejileri DI ile almak için (isteğe bağlı)
        private readonly AIModelScenarioStrategy _aiStrategy;
        private readonly RuleBasedScenarioStrategy _ruleStrategy;


        public ScenariosController(ScenarioService scenarioService,
                                   AIModelScenarioStrategy aiStrategy,
                                   RuleBasedScenarioStrategy ruleStrategy)
        {
            _scenarioService = scenarioService;
            _aiStrategy = aiStrategy;
            _ruleStrategy = ruleStrategy;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScenario(int id)
        {
            var scenario = await _scenarioService.GetScenarioByIdAsync(id);
            if (scenario == null) return NotFound();
            return Ok(scenario);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScenarios()
        {
            var scenarios = await _scenarioService.GetAllScenariosAsync();
            return Ok(scenarios);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateScenario([FromBody] StudentProfile studentProfile, [FromQuery] string strategyType = "rule")
        {
            if (studentProfile == null) return BadRequest("Student profile is required.");

            // Dinamik strateji seçimi
            if (strategyType.ToLower() == "ai")
            {
                _scenarioService.SetScenarioGenerationStrategy(_aiStrategy);
            }
            else // Varsayılan veya "rule"
            {
                _scenarioService.SetScenarioGenerationStrategy(_ruleStrategy);
            }

            var scenario = await _scenarioService.CreatePersonalizedScenarioAsync(studentProfile);
            return CreatedAtAction(nameof(GetScenario), new { id = scenario.Id }, scenario);
        }
    }
}