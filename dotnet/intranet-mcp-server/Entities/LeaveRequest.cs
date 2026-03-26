using System.ComponentModel;

namespace intranet_mcp_server.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Type { get; set; } //(férias, folga, licença médica, ausência justificada)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } //(pendente, aprovado, rejeitado)
        public DateTime RequestedAt { get; set; }
        public int ApprovedById { get; set; }
    }
}
