using System;

namespace intranet_mcp_server.Entities
{
    public class ProjectAllocation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RoleOnProject { get; set; } = string.Empty;

        // Navigation properties
        public Employee? Employee { get; set; }
        public Project? Project { get; set; }
    }
}
