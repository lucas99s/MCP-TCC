using System;
using intranet_mcp_server.Entities;

namespace intranet_mcp_server.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime BirthDate { get; set; }
        public string Status { get; set; } = string.Empty; //(ativo, afastado, desligado)
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? ManagerId { get; set; } //(self reference)

        // Navigation properties
        public Department? Department { get; set; }
        public Position? Position { get; set; }
    }
}
