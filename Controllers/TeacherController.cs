using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Models; // ApplicationUser için
using YetenekPusulasi.Models.ClassroomViewModels; // ViewModel'lar için

namespace YetenekPusulasi.Controllers
{
    [Authorize(Roles = "Teacher")] // Sadece Teacher rolündeki kullanıcılar erişebilir
    public class TeacherController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(IClassroomService classroomService, UserManager<ApplicationUser> userManager)
        {
            _classroomService = classroomService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var classrooms = await _classroomService.GetClassroomsByTeacherAsync(teacherId);
            // Gerekirse ViewModel'a map et
            return View(classrooms); // Views/Teacher/Dashboard.cshtml
        }

        [HttpGet]
        public IActionResult CreateClassroom()
        {
            return View(new CreateClassroomViewModel()); // Views/Teacher/CreateClassroom.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClassroom(CreateClassroomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var classroom = await _classroomService.CreateClassroomAsync(model.Name, model.Description, teacherId);
                TempData["SuccessMessage"] = $"'{classroom.Name}' sınıfı başarıyla oluşturuldu. Katılım Kodu: {classroom.ParticipationCode}";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ClassroomDetails(int id)
        {
            var classroom = await _classroomService.GetClassroomByIdAsync(id);
            if (classroom == null || classroom.TeacherId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound(); // Veya yetkisiz erişim sayfası
            }
            var students = await _classroomService.GetStudentsInClassroomAsync(id);
            ViewBag.Students = students; // Öğrencileri ViewBag ile taşı
            return View(classroom); // Views/Teacher/ClassroomDetails.cshtml
        }

         // Senaryo yönetimi için ScenariosController'a yönlendirme linkleri Dashboard'da olabilir
    }
}