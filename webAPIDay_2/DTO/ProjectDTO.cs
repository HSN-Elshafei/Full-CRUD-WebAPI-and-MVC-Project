using webAPIDay_2.Models;
namespace webAPIDay_2.DTO
{
    public class ProjectDTO
    {
        public int ProjId { get; set; }
        public string ProjName { get; set; }
        public string EmpsName { get; set; }
        public ICollection<String> Employees { get; set; }
    }
}
