namespace intranet_mcp_server.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int ProjectManagerId { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public Employee? ProjectManager { get; set; }
    }
}
