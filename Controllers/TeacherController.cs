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

using YetenekPusulasi.Models.ClassroomViewModels;                      // AppRoles için (eğer varsa)

namespace YetenekPusulasi.WebApp.Controllers // Namespace'inizi kontrol edin
{
    [Authorize(Roles = "Teacher")] // Veya "Teacher" string'i
    public class TeacherController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScenarioService _scenarioService;

        public TeacherController(IClassroomService classroomService, UserManager<ApplicationUser> userManager, IScenarioService scenarioService)
        {
            _classroomService = classroomService;
            _userManager = userManager;
            _scenarioService = scenarioService;
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
                Students = students.ToList(), // ToList() ile IEnumerable'ı List'e çeviriyoruz
                Scenarios = scenarios.ToList() // ToList() ile IEnumerable'ı List'e çeviriyoruz
            };

            return View(viewModel); // <<< DİKKAT: View'e viewModel gönderiliyor
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
                    teacherId,
                    model.ClassroomId);

                if (createdScenario != null)
                {
                    TempData["SuccessMessage"] = $"'{createdScenario.Title}' senaryosu '{classroom.Name}' sınıfına başarıyla eklendi.";
                    return RedirectToAction(nameof(ClassroomDetails), new { id = model.ClassroomId });
                }
                else
                {
                    ModelState.AddModelError("", "Senaryo oluşturulurken bir hata oluştu veya yetkiniz yok.");
                }
            }
            // Hata durumunda ViewModel'ı tekrar doldur
            model.ScenarioTypes = Enum.GetValues(typeof(ScenarioType)).Cast<ScenarioType>().Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() }).ToList();
            return View(model);
        }
    }
}