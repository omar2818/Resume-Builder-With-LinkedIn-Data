using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Models;
using ResumeBuilder.Services;

namespace ResumeBuilder.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private readonly ResumeService _resumeService;
        private readonly LinkedInService _linkedInService;
        private readonly PdfService _pdfService;

        public ResumeController(ResumeService resumeService, LinkedInService linkedInService, PdfService pdfService)
        {
            _resumeService = resumeService;
            _linkedInService = linkedInService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);
            return View(resume);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Resume resume)
        {
            if (!ModelState.IsValid)
            {
                return View(resume);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            resume.UserId = userId;

            await _resumeService.SaveResume(resume);
            return RedirectToAction("Dashboard", "Home");
        }

        public async Task<IActionResult> ImportFromLinkedIn()
        {
            try
            {
                var profile = await _linkedInService.GetUserProfile();
                var resume = _resumeService.MapFromLinkedInProfile(profile);

                await _resumeService.SaveResume(resume);

                return RedirectToAction("Edit");
            }
            catch (System.Exception ex)
            {
                // Log the error
                return RedirectToAction("Error", "Home", new { message = "Failed to import from LinkedIn: " + ex.Message });
            }
        }

        public async Task<IActionResult> DownloadPdf()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            if (resume == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateResumePdf(resume);
            return File(pdfBytes, "application/pdf", $"Resume_{resume.FirstName}_{resume.LastName}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(Education education)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            resume.Education.Add(education);
            await _resumeService.SaveResume(resume);

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> AddExperience(Experience experience)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            resume.Experience.Add(experience);
            await _resumeService.SaveResume(resume);

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(Skill skill)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            resume.Skills.Add(skill);
            await _resumeService.SaveResume(resume);

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEducation(int index)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            if (index >= 0 && index < resume.Education.Count)
            {
                resume.Education.RemoveAt(index);
                await _resumeService.SaveResume(resume);
            }

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExperience(int index)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            if (index >= 0 && index < resume.Experience.Count)
            {
                resume.Experience.RemoveAt(index);
                await _resumeService.SaveResume(resume);
            }

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSkill(int index)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var resume = await _resumeService.GetResumeForUser(userId);

            if (index >= 0 && index < resume.Skills.Count)
            {
                resume.Skills.RemoveAt(index);
                await _resumeService.SaveResume(resume);
            }

            return RedirectToAction("Edit");
        }
    }
}
