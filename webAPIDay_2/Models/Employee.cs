using System.ComponentModel.DataAnnotations.Schema;

namespace webAPIDay_2.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}

