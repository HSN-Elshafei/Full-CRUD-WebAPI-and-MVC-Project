namespace MVCUsingAPI.Models
{
    public class ProjectCheckBoxItem
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public bool IsAssigned { get; set; } // Ensure this is a boolean
    }
}
