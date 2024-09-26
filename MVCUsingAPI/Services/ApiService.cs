using MVCUsingAPI.Models;

namespace MVCUsingAPI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Department>>("api/Department");
        }
        public async Task<Department> GetDepartmentsByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Department>($"api/Department/{id}");
        }

        public async Task<Department> CreateDepartmentAsync(AddDepartment departmentAddDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Department", departmentAddDTO);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Department>();
        }

        public async Task UpdateDepartmentAsync(int deptId, AddDepartment departmentAddDTO)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Department/{deptId}", departmentAddDTO);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteDepartmentAsync(int deptId)
        {
            var response = await _httpClient.DeleteAsync($"api/Department/{deptId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete department.");
            }
        }


        public async Task<List<Employee>> GetEmployeesInDepartmentAsync(int deptId)
        {
            return await _httpClient.GetFromJsonAsync<List<Employee>>($"api/Department/{deptId}/Employees");
        }

        public async Task<List<Employee>> GetEmpsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Employee>>("api/Employee");
        }
        public async Task<Employee> GetEmpsByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Employee>($"api/Employee/{id}");
        }
        public async Task<Employee> CreateEmpAsync(AddEmployee addEmployee)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Employee", addEmployee);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Employee>();
        }
        public async Task UpdateEmpAsync(int id, AddEmployee addEmp)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Employee/{id}", addEmp);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteEmpAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Employee/{id}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<Department>> GetProjsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Department>>("api/Project");
        }
        public async Task<List<Project>> GetEmployeeProjectsAsync(int employeeId)
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>($"api/Employee/{employeeId}/Projects");
        }
        public async Task AddProjectsToEmployeeAsync(int employeeId, List<int> projectIds)
        {
            await _httpClient.PutAsJsonAsync($"api/Employee/{employeeId}/AssignProjects", projectIds);
        }


        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>("api/Project");
        }

        public async Task RemoveProjectsFromEmployeeAsync(int employeeId, List<int> projectIds)
        {
            await _httpClient.PostAsJsonAsync($"api/Employee/{employeeId}/RemoveProjects", projectIds);
        }



        public async Task<Project> CreateProjAsync(AddProject addProj)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Project", addProj);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Project>();
        }
        public async Task UpdateProjAsync(int deptId, AddProject addProj)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Project/{deptId}", addProj);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteProjAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Project/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
