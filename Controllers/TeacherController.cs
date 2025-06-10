// Controllers/TeacherController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Services; // IClassroomService ve IScenarioService için
using YetenekPusulasi.Data;                     // ApplicationUser için
// YetenekPusulasi.Models namespace'ini artık kullanmıyoruz gibi görünüyor ApplicationUser için, Data altında.
using YetenekPusulasi.WebApp.Models.ClassroomViewModels; // CreateClassroomViewModel ve ClassroomDetailsViewModel için
using YetenekPusulasi.WebApp.Models.ScenarioViewModels;  // CreateScenarioViewModel için
using YetenekPusulasi.Core.Entities;                     // Scenario, Classroom, ScenarioType için
using Microsoft.AspNetCore.Mvc.Rendering;                // SelectListItem için

using YetenekPusulasi.Models.ClassroomViewModels;
using YetenekPusulasi.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;                      // AppRoles için (eğer varsa)

namespace YetenekPusulasi.WebApp.Controllers // Namespace'inizi kontrol edin
{
    [Authorize(Roles = "Teacher")] // Veya "Teacher" string'i
    public class TeacherController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScenarioService _scenarioService;
        private readonly ILogger<TeacherController> _logger; // Loglama için
        private readonly ApplicationDbContext _context; // DbContext'i kullanmak için
        private readonly IAiConnectionTester _aiConnectionTester;

        public TeacherController(IClassroomService classroomService,
        UserManager<ApplicationUser> userManager,
        IScenarioService scenarioService,
        ILogger<TeacherController> logger,
        ApplicationDbContext context, IAiConnectionTester aiConnectionTester)
        {
            _classroomService = classroomService;
            _userManager = userManager;
            _scenarioService = scenarioService;
            _logger = logger;
            _context = context;
            _aiConnectionTester = aiConnectionTester;
        }

        public async Task<IActionResult> Dashboard()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized(); // Ekstra kontrol

            var classrooms = await _classroomService.GetClassroomsByTeacherAsync(teacherId);
            return View(classrooms);
        }

        [HttpGet]
        public IActionResult CreateClassroom()
        {
            return View(new CreateClassroomViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClassroom(CreateClassroomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(teacherId)) return Unauthorized(); // Ekstra kontrol

                var classroom = await _classroomService.CreateClassroomAsync(model.Name, model.Description, teacherId);
                if (classroom != null) // Servis null dönebilir (hatalı durum vb.)
                {
                    TempData["SuccessMessage"] = $"'{classroom.Name}' sınıfı başarıyla oluşturuldu. Katılım Kodu: {classroom.ParticipationCode}";
                    return RedirectToAction(nameof(Dashboard));
                }
                else
                {
                    ModelState.AddModelError("", "Sınıf oluşturulurken bir hata oluştu.");
                }
            }
            return View(model);
        }

        //----------------------------------------------------//
        // <<< HATA ALDIĞINIZ METODUN DÜZELTİLMİŞ HALİ >>>    //
        //----------------------------------------------------//
        [HttpGet]
        public async Task<IActionResult> ClassroomDetails(int id) // id classroomId'dir
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var classroom = await _classroomService.GetClassroomByIdAsync(id);

            // Yetki kontrolü: Sınıf var mı ve bu öğretmene mi ait?
            if (classroom == null || classroom.TeacherId != teacherId)
            {
                TempData["ErrorMessage"] = "Sınıf bulunamadı veya bu sınıfa erişim yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard)); // Veya NotFound()
            }

            var students = await _classroomService.GetStudentsInClassroomAsync(id);
            var scenarios = await _scenarioService.GetScenariosByClassroomAsync(id);

            var viewModel = new ClassroomDetailsViewModel
            {
                Classroom = classroom,
                Students = students.ToList(),
                Scenarios = scenarios.ToList() // scenarios.ToList() -> List<IScenario> döndürür ve bu IList<IScenario>'ya atanabilir.
            };
            return View(viewModel);
        }
        //----------------------------------------------------//
        // <<< DÜZELTME BİTTİ >>>                             //
        //----------------------------------------------------//


        [HttpGet]
        public async Task<IActionResult> CreateScenarioForClassroom(int classroomId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var classroom = await _classroomService.GetClassroomByIdAsync(classroomId);
            if (classroom == null || classroom.TeacherId != teacherId)
            {
                TempData["ErrorMessage"] = "Sınıf bulunamadı veya bu sınıfa senaryo ekleme yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new CreateScenarioViewModel
            {
                ClassroomId = classroomId,
                ClassroomName = classroom.Name
                // ScenarioTypes, ViewModel'ın constructor'ında dolduruluyor
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScenarioForClassroom(CreateScenarioViewModel model)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            // Yeniden sınıf ve yetki kontrolü (güvenlik için iyi bir pratik)
            var classroom = await _classroomService.GetClassroomByIdAsync(model.ClassroomId);
            if (classroom == null || classroom.TeacherId != teacherId)
            {
                ModelState.AddModelError("", "Geçersiz sınıf veya yetkisiz işlem.");
                // ViewModel'ı tekrar doldurmak gerekebilir postback için (ScenarioTypes vs.)
                model.ScenarioTypes = Enum.GetValues(typeof(ScenarioType)).Cast<ScenarioType>().Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() }).ToList();
                model.ClassroomName = classroom?.Name; // Sınıf adı null olabilir, kontrol et
                return View(model);
            }
            model.ClassroomName = classroom.Name;


            if (ModelState.IsValid)
            {
                var createdScenario = await _scenarioService.CreateScenarioAsync(
                    model.Title,
                    model.Description,
                    model.Type,
                    teacherId, // Doğru teacherId
                    model.ClassroomId,
                    model.TeacherProvidedInitialPrompt, // <<< YENİ
                    model.GenerateInitialPromptWithAI   // <<< YENİ
                );

                if (createdScenario != null)
                {
                    TempData["SuccessMessage"] = $"'{createdScenario.Title}' senaryosu '{classroom.Name}' sınıfına başarıyla eklendi. Başlangıç Prompt'u: {(createdScenario.WasInitialPromptAIGenerated ? "AI tarafından oluşturuldu." : "Manuel girildi.")}";
                    return RedirectToAction(nameof(ClassroomDetails), new { id = model.ClassroomId });
                }
                // ... (hata yönetimi aynı) ...
            }
            // Hata durumunda ViewModel'ı tekrar doldur
            model.ScenarioTypes = Enum.GetValues(typeof(ScenarioType)).Cast<ScenarioType>().Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() }).ToList();
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> ViewStudentAnswerAnalysis(int studentAnswerId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            // Analiz sonucunu çek ve öğretmenin bu cevabı görme yetkisi olup olmadığını kontrol et.
            // (Cevabın ait olduğu senaryonun sınıfının öğretmeni mi?)
            var analysis = await _context.AnalysisResults
                .Include(ar => ar.StudentAnswer)
                    .ThenInclude(sa => sa.Student) // Öğrenci bilgisini de al
                .Include(ar => ar.StudentAnswer)
                    .ThenInclude(sa => sa.Scenario) // Senaryo bilgisini de al
                        .ThenInclude(s => s.Classroom) // Senaryo üzerinden sınıfı da al
                .FirstOrDefaultAsync(ar => ar.StudentAnswerId == studentAnswerId);

            if (analysis == null)
            {
                TempData["ErrorMessage"] = "Analiz sonucu bulunamadı.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Yetki kontrolü: Bu senaryonun sınıfının öğretmeni mi?
            if (analysis.StudentAnswer?.Scenario?.Classroom?.TeacherId != teacherId)
            {
                TempData["ErrorMessage"] = "Bu analiz sonucunu görüntüleme yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (analysis.StudentAnswer == null || analysis.StudentAnswer.Scenario == null || analysis.StudentAnswer.Student == null)
            {
                _logger.LogError("ViewStudentAnswerAnalysis: AnalysisResult (ID: {AnalysisId}) için ilişkili entity'ler yüklenemedi.", analysis.Id);
                TempData["ErrorMessage"] = "Analiz detayı yüklenirken bir sorun oluştu.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(analysis); // Views/Teacher/ViewStudentAnswerAnalysis.cshtml
        }




        [HttpGet]
        public async Task<IActionResult> ViewScenarioAnswers(int scenarioId)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var scenario = await _scenarioService.GetScenarioByIdAsync(scenarioId);
            if (scenario == null) return NotFound("Senaryo bulunamadı.");

            // Yetki Kontrolü: Öğretmen bu senaryonun (ait olduğu sınıf üzerinden) sahibi mi?
            if (scenario.TeacherId != teacherId && (scenario.Classroom == null || scenario.Classroom.TeacherId != teacherId))
            {
                // Senaryonun TeacherId'si direkt öğretmene ait değilse ve sınıfın öğretmeni de değilse yetki verme.
                // Genellikle scenario.TeacherId yeterli olur, ama sınıf bazlı senaryolarda classroom.TeacherId de kontrol edilebilir.
                // Veya en iyisi: scenario'yu çekerken zaten öğretmenin yetkili olduğu senaryoları çekmek.
                // Şimdilik basit bir kontrol:
                if (scenario.TeacherId != teacherId)
                {
                    var classroom = await _classroomService.GetClassroomByIdAsync(scenario.ClassroomId);
                    if (classroom == null || classroom.TeacherId != teacherId)
                    {
                        TempData["ErrorMessage"] = "Bu senaryonun cevaplarını görüntüleme yetkiniz yok.";
                        return RedirectToAction(nameof(Dashboard));
                    }
                }
            }

            // Bu senaryoya verilmiş tüm cevapları çek (StudentAnswer ve ilişkili Student ile AnalysisResult'ı da al)
            var studentAnswers = await _context.StudentAnswers // VEYA _studentAnswerService.GetAnswersByScenarioAsync(scenarioId)
                .Where(sa => sa.ScenarioId == scenarioId)
                .Include(sa => sa.Student) // Öğrenci bilgisini al
                .Include(sa => sa.AnalysisResult) // Analiz sonucunu al (varsa)
                .OrderByDescending(sa => sa.SubmissionDate)
                .ToListAsync();

            ViewBag.ScenarioTitle = scenario.Title;
            ViewBag.ScenarioId = scenarioId; // Geri linki için veya başka işlemler için

            return View(studentAnswers); // Views/Teacher/ViewScenarioAnswers.cshtml
        }

        



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestGeminiConnection()
        {
            // Model identifier'ı GoogleGeminiAdapter'ınızdaki ile aynı yapın
            var (isSuccess, message) = await _aiConnectionTester.TestConnectionAsync("Google-gemini-2.0-flash"); // VEYA "Google-Gemini-Mock" veya ModelIdentifier'ınız ne ise o

            if (isSuccess)
            {
                TempData["SuccessMessage"] = $"Gemini API Test: {message}";
            }
            else
            {
                TempData["ErrorMessage"] = $"Gemini API Test: {message}";
            }
            // Kullanıcıyı, test butonunun olduğu sayfaya geri yönlendirin.
            // Örneğin, senaryo listeleme veya öğretmen dashboard'ı.
            // Şimdilik Dashboard'a yönlendirelim.
            return RedirectToAction(nameof(Dashboard)); // VEYA return PartialView("_TestResultMessage", message); AJAX ile yapılıyorsa
        }
    }
}