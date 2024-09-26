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
    public class ProjectController : ControllerBase
    {
        private readonly ITIContext _context;

        public ProjectController(ITIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Display()
        {
            var projects = await _context.Project
                .Include(p => p.Employees).Select(e => new ProjectDTO
                {
                    ProjId = e.Id,
                    ProjName = e.Name,
                    EmpsName = "",
                    Employees = e.Employees.Select(e=>e.Name).ToList(),
                }) 
                .ToListAsync();
            return Ok(projects);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> DisplayById(int id)
        {
            var project = await _context.Project
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            return Ok(new ProjectDTO
            {
                ProjId = project.Id,
                ProjName = project.Name,
                EmpsName = "",
                Employees = project.Employees.Select(e => e.Name).ToList(),
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectAddDTO projectAddDTO)
        {
            if (!ModelState.IsValid || projectAddDTO == null || string.IsNullOrWhiteSpace(projectAddDTO.ProjName))
            {
                return BadRequest("Invalid input data.");
            }

            var project = new Project
            {
                Name = projectAddDTO.ProjName
            };



            try
            {
                await _context.Project.AddAsync(project);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(DisplayById), new { id = project.Id }, new ProjectDTO
                {
                    ProjId = project.Id,
                    ProjName = project.Name
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ProjectAddDTO projectAddDTO)
        {
            if (!ModelState.IsValid || projectAddDTO == null || string.IsNullOrWhiteSpace(projectAddDTO.ProjName))
            {
                return BadRequest("Invalid input data.");
            }

            var project = await _context.Project
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            project.Name = projectAddDTO.ProjName;


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
            var project = await _context.Project.FindAsync(id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            _context.Project.Remove(project);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error deleting project: {ex.Message}");
            }
        }

        [HttpGet("{id:int}/Employees")]
        public async Task<IActionResult> GetProjectEmployees(int id)
        {
            var project = await _context.Project
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            var projectEmployees = project.Employees
                .Select(e => new EmployeeDTO
                {
                    EmpId = e.Id,
                    EmpName = e.Name
                })
                .ToList();

            return Ok(projectEmployees);
        }

        [HttpPost("{id:int}/RemoveEmployees")]
        public async Task<IActionResult> RemoveEmployees(int id, List<int> employeeIds)
        {
            var project = await _context.Project.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }
            foreach (var employeeId in employeeIds)
            {
                var employee = project.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee != null)
                {
                    project.Employees.Remove(employee);
                }
            }
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpPut("{projectId:int}/AssignEmployees")]
        public async Task<IActionResult> AssignEmployeesToProject(int projectId, [FromBody] List<int> employeeIds)
        {
            var project = await _context.Project
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            var employees = await _context.Employee
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            if (employees == null || !employees.Any())
            {
                return BadRequest("No valid employees provided.");
            }

            project.Employees.Clear();

            foreach (var employee in employees)
            {
                project.Employees.Add(employee);
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


    }
}
