// Core/Facades/StudentEnrollmentFacade.cs
using YetenekPusulasi.Core.Services;
using YetenekPusulasi.Core.Entities;
using System.Threading.Tasks;
namespace YetenekPusulasi.Core.Facades
{
    public class StudentEnrollmentFacade
    {
        private readonly ClassroomManagementService _classroomService;
        private readonly ScenarioOrchestrationService _scenarioService;
        // Belki IEmailService gibi başka servisler de olabilir

        public StudentEnrollmentFacade(ClassroomManagementService classroomService, ScenarioOrchestrationService scenarioService)
        {
            _classroomService = classroomService;
            _scenarioService = scenarioService;
        }

        public async Task<(bool success, string message, Classroom classroom, Scenario initialScenario)>
            EnrollStudentAndAssignFirstScenarioAsync(string studentId, string participationCode)
        {
            bool joined = await _classroomService.JoinStudentToClassroomAsync(studentId, participationCode);
            if (!joined)
            {
                return (false, "Sınıfa katılım başarısız.", null, null);
            }

            var classroom = await _classroomService.GetByParticipationCodeAsync(participationCode);
            // Öğrenciye varsayılan bir senaryo ata (basit bir örnek)
            var initialScenario = await _scenarioService.CreateAndStoreScenarioAsync(classroom.Name + " Giriş", ScenarioDifficulty.Easy);

            // Burada öğrenciye e-posta gönderme gibi ek adımlar olabilir.

            return (true, "Öğrenci sınıfa katıldı ve ilk senaryo atandı.", classroom, initialScenario);
        }
    }
}