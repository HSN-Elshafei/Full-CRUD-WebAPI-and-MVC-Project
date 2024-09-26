using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCUsingAPI.Models;
using MVCUsingAPI.Services;

namespace MVCUsingAPI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApiService _apiService;

        public EmployeeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Display all Employees
        public async Task<IActionResult> Index()
        {
            var emps = await _apiService.GetEmpsAsync();
            return View(emps);
        }

        // GET: Create a new Employee
        public async Task<IActionResult> Create()
        {
            var emps = await _apiService.GetDepartmentsAsync();

            var depts = emps
                .Select(i => new SelectListItem
                {
                    Value = i.DeptId.ToString(),
                    Text = i.DeptName
                })
                .ToList();

            ViewBag.DeptEmps = depts;
            return View();
        }

        // POST: Create a new Employee
        [HttpPost]
        public async Task<IActionResult> Create(AddEmployee addEmp)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateEmpAsync(addEmp);
                return RedirectToAction(nameof(Index));
            }
            return View(addEmp);
        }

        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var emp = await _apiService.GetEmpsByIdAsync(id);

            if (emp == null)
            {
                return NotFound();
            }

            var depts = await _apiService.GetDepartmentsAsync();
            int departmentId = depts.FirstOrDefault(dep => dep.DeptName == emp.EmpDept)?.DeptId ?? 0;

            AddEmployee addEmployee = new AddEmployee()
            {
                EmpId = emp.EmpId,
                EmpName = emp.EmpName,
                DeptID = departmentId
            };

            ViewBag.DeptEmps = depts
                .Select(i => new SelectListItem
                {
                    Value = i.DeptId.ToString(),
                    Text = i.DeptName
                })
                .ToList();

            ViewBag.ReturnUrl = returnUrl; 

            return View(addEmployee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddEmployee addEmp)
        {
            if (ModelState.IsValid)
            {
                await _apiService.UpdateEmpAsync(id, addEmp);
                return RedirectToAction(nameof(Index));
            }
            return View(addEmp);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            try
            {
                await _apiService.DeleteEmpAsync(id);
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = "You can't delete this Employee because it has Projects assigned.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> AssignProjects(int id)
        {
            var employee = await _apiService.GetEmpsByIdAsync(id);
            if (employee == null)
            {
                return NotFound(); 
            }
            var allProjects = await _apiService.GetProjectsAsync();

            var assignedProjects = await _apiService.GetEmployeeProjectsAsync(id);

            var viewModel = new EmployeeProject
            {
                EmployeeId = employee.EmpId,
                EmployeeName = employee.EmpName, 
                Projects = allProjects.Select(proj => new ProjectCheckBoxItem
                {
                    ProjectId = proj.ProjId,
                    ProjectName = proj.ProjName,
                    IsAssigned = assignedProjects.Any(ap => ap.ProjId == proj.ProjId)
                }).ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AssignProjects(EmployeeProject viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Key: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(viewModel);
            }
            var currentProjects = await _apiService.GetEmployeeProjectsAsync(viewModel.EmployeeId);
            var selectedProjectIds = viewModel.Projects
                .Where(p => p.IsAssigned)
                .Select(p => p.ProjectId)
                .ToList();

            var projectsToRemove = currentProjects.Select(cp => cp.ProjId).Except(selectedProjectIds).ToList();

            if (selectedProjectIds.Any())
            {
                await _apiService.AddProjectsToEmployeeAsync(viewModel.EmployeeId, selectedProjectIds);
            }

            if (projectsToRemove.Any())
            {
                await _apiService.RemoveProjectsFromEmployeeAsync(viewModel.EmployeeId, projectsToRemove);
            }
            return RedirectToAction("Index");
        }


    }
}
