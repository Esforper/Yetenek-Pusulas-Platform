using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.Services;
using YetenekPusulasi.Models.StudentViewModels; // ViewModel'lar için

namespace YetenekPusulasi.Controllers
{
    [Authorize(Roles = "Student")] // Sadece Student rolündeki kullanıcılar erişebilir
    public class StudentController : Controller
    {
        private readonly IClassroomService _classroomService;
        // IScenarioService de buraya enjekte edilebilir

        public StudentController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        public IActionResult Dashboard()
        {
            // Öğrencinin katıldığı sınıfları, atanmış senaryoları vb. listele
            // Bu kısım daha sonra doldurulacak
            return View(); // Views/Student/Dashboard.cshtml
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
    }
}