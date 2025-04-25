using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Models;
using ResumeBuilder.Models.ViewModels;
using ResumeBuilder.Services;

namespace ResumeBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ResumeService _resumeService;

        public HomeController(ResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var resume = await _resumeService.GetResumeForUser(userId);
            var viewModel = new ResumeViewModel
            {
                Resume = resume,
                IsLinkedInImported = resume.FirstName != null
            };

            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> LoginWithLinkedIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Dashboard") }, "LinkedIn");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}
