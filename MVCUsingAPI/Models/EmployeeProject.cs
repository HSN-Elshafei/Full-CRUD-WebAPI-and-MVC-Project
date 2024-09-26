using System.ComponentModel.DataAnnotations;

namespace MVCUsingAPI.Models
{
    public class EmployeeProject
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<ProjectCheckBoxItem> Projects { get; set; }
    }
}
