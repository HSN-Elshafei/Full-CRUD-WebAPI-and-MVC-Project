using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using webAPIDay_2.Models;
using webAPIDay_2.DTO;

namespace webAPIDay_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ITIContext _context;

        public EmployeeController(ITIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Display()
        {
            var employees = await _context.Employee
                .Include(e => e.Projects) 
                .ToListAsync();

            string proNames = "";
            List<EmployeeDTO> employeesDTO = new List<EmployeeDTO>();
            foreach (var emp in employees)
            {
                foreach (var pro in emp.Projects)
                {
                    proNames +=  pro.Name+", ";
                }
                employeesDTO.Add(new EmployeeDTO
                {
                    EmpId = emp.Id,
                    EmpName = emp.Name,
                    EmpDept = _context.Department.Find(emp.DepartmentId).Name,
                    ProjName = proNames
                });
                proNames = "";
            }

            

            return Ok(employeesDTO);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> DisplayById(int id)
        {
            var employee = await _context.Employee.Include(e => e.Projects).FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            string proNames = "";
            foreach (var pro in employee.Projects)
            {
                proNames += pro.Name+ ", ";
            }
            EmployeeDTO employeeDTO = new EmployeeDTO
            {
                EmpId = employee.Id,
                EmpName = employee.Name,
                EmpDept = _context.Department.Find(employee.DepartmentId).Name,
                ProjName = proNames
            };

            return Ok(employeeDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeAddDTO employeeAddDTO)
        {
            if (!ModelState.IsValid || employeeAddDTO == null || string.IsNullOrWhiteSpace(employeeAddDTO.EmpName))
            {
                return BadRequest("Invalid input data.");
            }


            var employee = new Employee
            {
                Name = employeeAddDTO.EmpName,
                DepartmentId = employeeAddDTO.DeptID,
            };

            try
            {
                await _context.Employee.AddAsync(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(DisplayById), new { id = employee.Id }, new EmployeeDTO
                {
                    EmpId = employee.Id,
                    EmpName = employee.Name,
                    EmpDept = _context.Department.Find(employee.DepartmentId).Name
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EmployeeAddDTO employeeAddDTO)
        {
            if (!ModelState.IsValid || employeeAddDTO == null || string.IsNullOrWhiteSpace(employeeAddDTO.EmpName))
            {
                return BadRequest("Invalid input data.");
            }

            var employee = await _context.Employee.Include(e => e.Projects).FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            employee.Name = employeeAddDTO.EmpName;
            employee.DepartmentId = employeeAddDTO.DeptID;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Concurrency error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            _context.Employee.Remove(employee);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error deleting employee: {ex.Message}");
            }
        }

        [HttpPut("{employeeId:int}/AssignProjects")]
        public async Task<IActionResult> AssignProjectsToEmployee(int employeeId, [FromBody] List<int> projectIds)
        {
            var employee = await _context.Employee
                .Include(e => e.Projects) 
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }

            var projects = await _context.Project
                .Where(p => projectIds.Contains(p.Id))
                .ToListAsync();

            if (projects == null || !projects.Any())
            {
                return BadRequest("No valid projects provided.");
            }

            employee.Projects.Clear();
            foreach (var project in projects)
            {
                employee.Projects.Add(project);
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id:int}/RemoveProjects")]
        public async Task<IActionResult> RemoveProjects(int id, List<int> projectIds)
        {
            var employee = await _context.Employee.Include(e => e.Projects).FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound($"Employee with ID {id} not found.");

            foreach (var projectId in projectIds)
            {
                var project = employee.Projects.FirstOrDefault(p => p.Id == projectId);
                if (project != null)
                {
                    employee.Projects.Remove(project);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}/Projects")]
        public async Task<IActionResult> GetEmployeeProjects(int id)
        {
            var employee = await _context.Employee
                .Include(e => e.Projects)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var employeeProjects = employee.Projects
                .Select(p => new ProjectDTO
                {
                    ProjId = p.Id,
                    ProjName = p.Name
                }).ToList();

            return Ok(employeeProjects);
        }

    }
}
