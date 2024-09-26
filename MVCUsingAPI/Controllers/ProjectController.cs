using Microsoft.AspNetCore.Mvc;
using MVCUsingAPI.Models;
using MVCUsingAPI.Services;

namespace MVCUsingAPI.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApiService _apiService;

        public ProjectController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Create a new Project
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddProject addProj)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateProjAsync(addProj);
                return RedirectToAction("Index", "Employee");
            }
            return View(addProj);
        }

        // GET: Edit Project
        public async Task<IActionResult> Edit(int id)
        {
            var projects = await _apiService.GetProjectsAsync();
            var project = projects.FirstOrDefault(p => p.ProjId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddProject addProj)
        {
            if (!ModelState.IsValid)
            {
                return View(addProj);
            }

            await _apiService.UpdateProjAsync(id, addProj);


            return RedirectToAction("Index", "Employee");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            try
            {
                await _apiService.DeleteProjAsync(id);
                TempData["SuccessMessage"] = "Project deleted successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = "You can't delete this Project because it has Employees assigned.";
            }

            return RedirectToAction("Index","Employee");
        }
    }

}
