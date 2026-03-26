namespace intranet_mcp_server.Entities
{
    public class PayrollSummary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ReferenceMonth { get; set; }   // 1-12
        public int ReferenceYear { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty; // (pendente, pago)
    }
}
