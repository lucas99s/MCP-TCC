using System.Reflection.PortableExecutable;

namespace intranet_mcp_server.Entities
{
    public class TimeEntry
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime EntryDate { get; set; }
        public double HoursWorked { get; set; }
        public string Description { get; set; } = string.Empty;
        public string EntryType { get; set; } = string.Empty; //(normal, extra, ausência justificada)
        public int ApprovedById { get; set; }

        // Navigation properties
        public Employee? Employee { get; set; }
        public Project? Project { get; set; }
    }
}
