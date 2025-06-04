using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Data;
using YetenekPusulasi.Models.StudentViewModels; // ViewModel'lar için

namespace YetenekPusulasi.Controllers
{
    [Authorize(Roles = "Student")] // Sadece Student rolündeki kullanıcılar erişebilir
    public class StudentController : Controller
    {
        private readonly IClassroomService _classroomService;
        // IScenarioService de buraya enjekte edilebilir
        private readonly IScenarioService _scenarioService;
        private readonly UserManager<ApplicationUser> _userManager; // Gerekirse

        public StudentController(IClassroomService classroomService, IScenarioService scenarioService, UserManager<ApplicationUser> userManager)
        {
            _classroomService = classroomService;
            _scenarioService = scenarioService;
            _userManager = userManager;
        }

        // Öğrenci Paneli - Katıldığı Sınıfları Listeler
        public async Task<IActionResult> Dashboard()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var classrooms = await _classroomService.GetClassroomsByStudentAsync(studentId);
            ViewBag.StudentName = User.Identity?.Name; // Veya ApplicationUser'dan FirstName, LastName
            return View(classrooms); // Views/Student/Dashboard.cshtml
        }


        [HttpGet]
        public IActionResult JoinClassroom()
        {
            return View(new JoinClassroomViewModel()); // Views/Student/JoinClassroom.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinClassroom(JoinClassroomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var success = await _classroomService.AddStudentToClassroomAsync(studentId, model.ParticipationCode);
                if (success)
                {
                    var classroom = await _classroomService.GetClassroomByCodeAsync(model.ParticipationCode);
                    TempData["SuccessMessage"] = $"'{classroom?.Name}' sınıfına başarıyla katıldınız.";
                    return RedirectToAction(nameof(Dashboard));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz katılım kodu veya bir hata oluştu.");
                }
            }
            return View(model);
        }
        


        // Bir Sınıftaki Senaryoları Listeleme
        [HttpGet]
        public async Task<IActionResult> ViewClassroomScenarios(int classroomId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            // Yetki Kontrolü: Öğrenci bu sınıfa kayıtlı mı?
            var isEnrolled = await _classroomService.IsStudentEnrolledAsync(studentId, classroomId);
            if (!isEnrolled)
            {
                TempData["ErrorMessage"] = "Bu sınıfa erişim yetkiniz yok veya böyle bir sınıfa kayıtlı değilsiniz.";
                return RedirectToAction(nameof(Dashboard));
            }

            var classroom = await _classroomService.GetClassroomByIdAsync(classroomId);
            if (classroom == null) return NotFound();

            var scenarios = await _scenarioService.GetScenariosByClassroomAsync(classroomId);

            ViewBag.ClassroomName = classroom.Name;
            ViewBag.ClassroomId = classroomId; // Geri dönme linki vb. için

            // İleride öğrencinin hangi senaryoları tamamladığı bilgisi de eklenebilir.
            return View(scenarios.ToList()); // Views/Student/ViewClassroomScenarios.cshtml
        }


        // Bir Senaryonun Detayını Görüntüleme (Cevaplama Sonraki Adım)
        [HttpGet]
        public async Task<IActionResult> ViewScenario(int scenarioId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var scenario = await _scenarioService.GetScenarioByIdAsync(scenarioId);
            if (scenario == null) return NotFound();

            // Yetki Kontrolü: Öğrenci bu senaryonun ait olduğu sınıfa kayıtlı mı?
            var isEnrolledInScenarioClass = await _classroomService.IsStudentEnrolledAsync(studentId, scenario.ClassroomId);
            if (!isEnrolledInScenarioClass)
            {
                TempData["ErrorMessage"] = "Bu senaryoya erişim yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard)); // Veya ait olduğu sınıfın senaryo listesine
            }

            // Strategy Deseni Kullanımı (Sonraki adımda veya şimdi eklenebilir)
            // IScenarioDisplayStrategy displayStrategy = _strategyFactory.GetStrategy(scenario.Type);
            // ViewBag.ScenarioDisplayHtml = displayStrategy.GetDisplayHtml(scenario);
            // return View(scenario); // Model olarak IScenario gönderiliyor

            // Şimdilik basitçe senaryoyu gönderelim, View'da temel bilgileri gösteririz.
            return View(scenario); // Views/Student/ViewScenario.cshtml
        }
    }
}