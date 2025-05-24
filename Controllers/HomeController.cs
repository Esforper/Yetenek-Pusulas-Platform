using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YetenekPusulasi.Models;

namespace YetenekPusulasi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated) // Eğer kullanıcı giriş yapmışsa dashboard'a yönlendir
            {
                if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Dashboard", "Teacher");
                }
                else if (User.IsInRole("Student"))
                {
                    return RedirectToAction("Dashboard", "Student");
                }
            }
            return View(); // Views/Home/Index.cshtml (Tanıtım içeriği burada olacak)
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
