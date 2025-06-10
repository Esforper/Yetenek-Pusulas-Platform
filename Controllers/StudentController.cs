using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Areas.Identity.Data;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Data;
using YetenekPusulasi.Models.StudentViewModels; // ViewModel'lar için
using YetenekPusulasi.WebApp.Models.StudentViewModels;

namespace YetenekPusulasi.Controllers
{
    [Authorize(Roles = "Student")] // Sadece Student rolündeki kullanıcılar erişebilir
    public class StudentController : Controller
    {
        private readonly IClassroomService _classroomService;
        // IScenarioService de buraya enjekte edilebilir
        private readonly IScenarioService _scenarioService;
        private readonly UserManager<ApplicationUser> _userManager; // Gerekirse
        private readonly IAnalysisService _analysisService; // Cevap analizi için
        private readonly ApplicationDbContext _context; // DbContext, eğer gerekli ise
        private readonly ILogger<StudentController> _logger; // Loglama için
        private readonly IStudentAnswerService _studentAnswerService; // Öğrenci cevaplarını yönetmek için


        public StudentController(IClassroomService classroomService,
        IScenarioService scenarioService,
        UserManager<ApplicationUser> userManager,
        IAnalysisService analysisService,
        ApplicationDbContext context,
        ILogger<StudentController> logger,
        IStudentAnswerService studentAnswerService)
        {
            _classroomService = classroomService;
            _scenarioService = scenarioService;
            _userManager = userManager;
            _analysisService = analysisService;
            _context = context;
            _logger = logger;
            _studentAnswerService = studentAnswerService;
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



        [HttpGet]
        public async Task<IActionResult> ViewScenario(int scenarioId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var scenario = await _scenarioService.GetScenarioByIdAsync(scenarioId);
            if (scenario == null) return NotFound();

            var isEnrolled = await _classroomService.IsStudentEnrolledAsync(studentId, scenario.ClassroomId);
            if (!isEnrolled)
            {
                TempData["ErrorMessage"] = "Bu senaryoya erişim yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Öğrencinin bu senaryoya verdiği cevapları çek
            var studentAnswersForThisScenario = await _studentAnswerService.GetAnswersByScenarioAsync(scenarioId, studentId);

            var viewModel = new ViewScenarioViewModel // Yeni bir ViewModel oluşturalım
            {
                Scenario = scenario,
                StudentAnswers = studentAnswersForThisScenario.ToList(),
                // Strategy deseni ile oluşturulacak HTML için (opsiyonel):
                // DisplayHtml = _strategyFactory.GetStrategy(scenario.Type).GetDisplayHtml(scenario)
            };

            // Cevap verme formunu sadece daha önce cevap verilmemişse veya güncellemeye izin veriliyorsa göster
            ViewBag.CanSubmitAnswer = !studentAnswersForThisScenario.Any(); // Basitçe: Eğer hiç cevap yoksa göster

            return View(viewModel); // Views/Student/ViewScenario.cshtml
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswer(SubmitAnswerViewModel model)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ModelState.IsValid kontrolü ve studentId null kontrolü en başta olmalı
            if (!ModelState.IsValid || string.IsNullOrEmpty(studentId))
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Lütfen geçerli bir cevap giriniz.";
                }
                if (string.IsNullOrEmpty(studentId))
                {
                    _logger.LogWarning("SubmitAnswer POST: Yetkisiz erişim denemesi veya studentId alınamadı.");
                    return Unauthorized(); // Veya RedirectToAction("Login", "Account")
                }

                // Hatalı durumda senaryoyu tekrar yükleyip ViewScenario'ya dönmek daha iyi olur.
                // Ancak bunun için senaryo bilgisini tekrar çekmek ve View'e göndermek gerekir.
                // Şimdilik basit bir yönlendirme:
                var scenarioForRedirect = await _scenarioService.GetScenarioByIdAsync(model.ScenarioId);
                if (scenarioForRedirect != null)
                {
                    // ViewScenario'ya model göndermek yerine ViewBag ile veya doğrudan View'e scenario gönderin
                    // Bu kısım ViewScenario GET metodunuzun nasıl çalıştığına bağlı.
                    // Eğer ViewScenario GET metodu sadece scenarioId alıyorsa:
                    return RedirectToAction("ViewScenario", new { scenarioId = model.ScenarioId });
                }
                return RedirectToAction("Dashboard"); // Genel bir fallback
            }

            var scenario = await _scenarioService.GetScenarioByIdAsync(model.ScenarioId);
            if (scenario == null)
            {
                _logger.LogWarning("SubmitAnswer POST: Senaryo bulunamadı. ScenarioId: {ScenarioId}", model.ScenarioId);
                TempData["ErrorMessage"] = "Senaryo bulunamadı.";
                return RedirectToAction("Dashboard");
            }

            var isEnrolled = await _classroomService.IsStudentEnrolledAsync(studentId, scenario.ClassroomId);
            if (!isEnrolled)
            {
                _logger.LogWarning("SubmitAnswer POST: Öğrenci {StudentId} bu senaryonun sınıfına ({ClassroomId}) kayıtlı değil.", studentId, scenario.ClassroomId);
                TempData["ErrorMessage"] = "Bu senaryoya cevap gönderme yetkiniz yok.";
                return RedirectToAction("Dashboard");
            }

            // TODO: Öğrencinin bu senaryoya daha önce cevap verip vermediğini kontrol et.
            // Eğer verdiyse, yeni cevap mı, güncelleme mi? Veya izin verme.
            // bool alreadyAnswered = await _context.StudentAnswers.AnyAsync(sa => sa.StudentId == studentId && sa.ScenarioId == model.ScenarioId);
            // if(alreadyAnswered) { TempData["WarningMessage"] = "Bu senaryoya daha önce cevap verdiniz."; return RedirectToAction(...); }


            var studentAnswer = new StudentAnswer // Değişken burada tanımlanıyor
            {
                ScenarioId = model.ScenarioId,
                StudentId = studentId,
                AnswerText = model.AnswerText,
                SubmissionDate = DateTime.UtcNow,
                Scenario = scenario as Scenario // Eğer StudentAnswer.Scenario IScenario değil de Scenario ise cast gerekebilir. IScenario ise direkt scenario atanabilir.
                                                // Veya Scenario property'sini hiç set etmeyin, sadece ScenarioId yeterli.
            };

            _context.StudentAnswers.Add(studentAnswer);
            await _context.SaveChangesAsync(); // Cevap kaydedildi, studentAnswer.Id oluştu.

            _logger.LogInformation("Öğrenci {StudentId}, Senaryo ID {ScenarioId} için cevap kaydetti. Cevap ID: {StudentAnswerId}", studentId, model.ScenarioId, studentAnswer.Id);
            TempData["SuccessMessage"] = "Cevabınız başarıyla gönderildi. Analiz ediliyor...";

            try // try-catch bloğu burada başlıyor
            {
                // studentAnswer ve scenario değişkenleri bu scope'ta erişilebilir.
                var analysisResult = await _analysisService.AnalyzeStudentAnswerAsync(studentAnswer, scenario, "Google-Gemini-Mock"); // Veya tercih edilen modeli dinamik al

                if (analysisResult != null && string.IsNullOrEmpty(analysisResult.ErrorMessage))
                {
                    _logger.LogInformation("Cevap ID {StudentAnswerId} başarıyla analiz edildi. Model: {AIModelUsed}", studentAnswer.Id, analysisResult.AiModelUsed);
                    TempData["SuccessMessage"] += " Cevabınız başarıyla analiz edildi!";
                    // Öğrenciyi analiz sonuçlarını görebileceği bir sayfaya yönlendir
                    return RedirectToAction("ViewAnalysisResult", new { studentAnswerId = studentAnswer.Id }); // Bu action'ı oluşturmanız gerekecek
                }
                else
                {
                    _logger.LogWarning("Cevap ID {StudentAnswerId} analizinde sorun oluştu. AI Model: {AIModelUsed}, Hata: {ErrorMessage}",
                        studentAnswer.Id, analysisResult?.AiModelUsed, analysisResult?.ErrorMessage);
                    TempData["WarningMessage"] = "Cevabınız kaydedildi ancak analiz sırasında bir sorun oluştu: " + (analysisResult?.ErrorMessage ?? "Bilinmeyen hata");
                }
            }
            catch (Exception ex) // catch bloğu burada
            {
                _logger.LogError(ex, "Cevap analizi tetiklenirken genel bir hata oluştu. Cevap ID: {AnswerId}", studentAnswer.Id);
                TempData["WarningMessage"] = "Cevabınız kaydedildi ancak analiz başlatılırken bir hata oluştu.";
            } // try-catch bloğu burada bitiyor

            // Hata durumunda veya analiz sonrası genel yönlendirme
            return RedirectToAction("ViewClassroomScenarios", new { classroomId = scenario.ClassroomId });
        }



        // Öğrencinin kendi analiz sonucunu görmesi için action
        [HttpGet]
        public async Task<IActionResult> ViewAnalysisResult(int studentAnswerId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            // Analiz sonucunu StudentAnswerId üzerinden çek. StudentAnswer'ı da include et.
            // StudentAnswer'dan da StudentId'yi kontrol et ki başkasının sonucunu görmesin.
            var analysis = await _context.AnalysisResults
                .Include(ar => ar.StudentAnswer) // Eğer AnalysisResult'ta StudentAnswer navigasyon property'si varsa
                                                 // .ThenInclude(sa => sa.Scenario) // Ve Scenario'yu da istiyorsak
                .FirstOrDefaultAsync(ar => ar.StudentAnswerId == studentAnswerId && ar.StudentAnswer.StudentId == studentId);

            if (analysis == null)
            {
                TempData["ErrorMessage"] = "Analiz sonucu bulunamadı veya bu sonuca erişim yetkiniz yok.";
                return RedirectToAction(nameof(Dashboard));
            }
            // Eğer AnalysisResult'ta StudentAnswer ve Scenario navigasyonları yoksa,
            // StudentAnswer'ı ayrıca çekip View'e göndermeniz gerekebilir.
            // var studentAnswer = await _context.StudentAnswers.Include(sa => sa.Scenario).FirstOrDefaultAsync(sa => sa.Id == studentAnswerId && sa.StudentId == studentId);
            // ViewBag.StudentAnswer = studentAnswer;

            return View(analysis); // Views/Student/ViewAnalysisResult.cshtml
        }
        




        [HttpGet]
        public async Task<IActionResult> MyProfile() // Veya MyAnswers
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null) return NotFound("Kullanıcı bulunamadı.");

            var allAnswers = await _studentAnswerService.GetAllAnswersByStudentAsync(studentId);

            var viewModel = new StudentProfileViewModel // Yeni bir ViewModel
            {
                UserName = student.UserName,
                Email = student.Email,
                SubmittedAnswers = allAnswers.ToList()
            };

            return View(viewModel); // Views/Student/MyProfile.cshtml
        }

        
    }
}