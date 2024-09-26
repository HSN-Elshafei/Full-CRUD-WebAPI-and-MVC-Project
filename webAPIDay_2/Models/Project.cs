using System.Text.Json.Serialization;

namespace webAPIDay_2.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
