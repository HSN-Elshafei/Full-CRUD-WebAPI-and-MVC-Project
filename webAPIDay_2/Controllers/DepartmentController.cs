using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using webAPIDay_2.DTO;
using webAPIDay_2.Models;

namespace webAPIDay_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ITIContext _context;

        public DepartmentController(ITIContext context)
        {
            _context = context;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<IActionResult> Display()
        {
            var departments = await _context.Department
                .Include(d => d.Employees)
                .ToListAsync();

            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }

            var departmentDTOs = departments.Select(dept => new DepartmentDTO
            {
                DeptId = dept.Id,
                DeptName = dept.Name,
                EmpCount = dept.Employees.Count()
            }).ToList();

            return Ok(departmentDTOs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DisplayById(int id)
        {
            var dept = await _context.Department.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
            if (dept == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            var deptDTO = new DepartmentDTO
            {
                DeptId = dept.Id,
                DeptName = dept.Name,
                EmpCount = dept.Employees.Count()
            };

            return Ok(deptDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentAddDTO departmentAddDTO)
        {
            if (!ModelState.IsValid || departmentAddDTO == null || string.IsNullOrWhiteSpace(departmentAddDTO.DeptName))
            {
                return BadRequest("Invalid input data.");
            }

            var department = new Department
            {
                Name = departmentAddDTO.DeptName
            };

            try
            {
                await _context.Department.AddAsync(department);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(DisplayById), new { id = department.Id }, department);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] DepartmentAddDTO departmentAddDTO)
        {
            if (!ModelState.IsValid || departmentAddDTO == null || string.IsNullOrWhiteSpace(departmentAddDTO.DeptName))
            {
                return BadRequest("Invalid input data.");
            }

            var department = await _context.Department.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            department.Name = departmentAddDTO.DeptName;

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
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            _context.Department.Remove(department);
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error deleting department: {ex.Message}");
            }
        }

        [HttpGet("{id:int}/Employees")]
        public async Task<IActionResult> GetEmployeesInDepartment(int id)
        {
            var department = await _context.Department
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            var employees = await _context.Employee.Include(e => e.Projects).Where(e => e.DepartmentId==id).ToListAsync();
            string proNames = "";
            List<EmployeeDTO> employeesDTO = new List<EmployeeDTO>();
            foreach (var emp in employees)
            {

                foreach (var pro in emp.Projects)
                {
                    proNames += pro.Name + ", ";
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
    }
}
