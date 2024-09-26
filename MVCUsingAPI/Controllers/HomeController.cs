using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCUsingAPI.Models;
using MVCUsingAPI.Services;
using System.Threading.Tasks;

namespace MVCUsingAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _apiService.GetDepartmentsAsync();

            return View(departments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddDepartment addDepartment)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateDepartmentAsync(addDepartment);
                return RedirectToAction(nameof(Index));
            }
            return View(addDepartment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var departments = await _apiService.GetDepartmentsAsync();
            var department = departments.FirstOrDefault(d => d.DeptId == id);
            if (department == null)
            {
                return NotFound();
            }

            

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddDepartment addDepartment)
        {
            if (ModelState.IsValid)
            {
                await _apiService.UpdateDepartmentAsync(id, addDepartment);
                return RedirectToAction(nameof(Index));
            }
            return View(addDepartment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteDepartmentAsync(id);
                TempData["SuccessMessage"] = "Department deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "You can't delete this department because it has employees assigned.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Employees(int id)
        {
            List<Employee> employees = await _apiService.GetEmployeesInDepartmentAsync(id);
            Department dept = await _apiService.GetDepartmentsByIdAsync(id);
            ViewBag.DepartmentName = dept.DeptName;
            ViewBag.DepartmentID = id;
            return View(employees);
        }

        public IActionResult CreateEmp(int id)
        {
            ViewBag.DepartmentID = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmp(AddEmployee addEmployee)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateEmpAsync(addEmployee);
                return RedirectToAction("Employees", new { id = addEmployee.DeptID });
            }
            return View(addEmployee);
        }


        public async Task<IActionResult> EditEmp(int id)
        {
            var emp = await _apiService.GetEmpsByIdAsync(id);

            var depts = await _apiService.GetDepartmentsAsync();
            int idd = 0;

            foreach (var dep in depts)
            {
                if (dep.DeptName == emp.EmpDept)
                {
                    idd = dep.DeptId;
                }
            }

            AddEmployee addEmployee = new AddEmployee()
            {
                EmpId = emp.EmpId,
                EmpName = emp.EmpName,
                DeptID = idd
            };

            var deptSelectList = depts
                .Select(i => new SelectListItem
                {
                    Value = i.DeptId.ToString(),
                    Text = i.DeptName
                })
                .ToList();

            ViewBag.DeptEmps = deptSelectList;

            if (emp == null)
            {
                return NotFound();
            }

            return View(addEmployee);
        }


        [HttpPost]
        public async Task<IActionResult> EditEmp(int id, AddEmployee addEmp)
        {
            if (ModelState.IsValid)
            {
                await _apiService.UpdateEmpAsync(id, addEmp);
                return RedirectToAction("Employees", new { id = addEmp.DeptID }) ;
            }
            return View(addEmp);
        }
    }
}
