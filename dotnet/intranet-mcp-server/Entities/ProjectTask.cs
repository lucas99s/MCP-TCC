using System;

namespace intranet_mcp_server.Entities
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AssignedEmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        // Navigation properties
        public Project? Project { get; set; }
        public Employee? AssignedEmployee { get; set; }
    }
}
