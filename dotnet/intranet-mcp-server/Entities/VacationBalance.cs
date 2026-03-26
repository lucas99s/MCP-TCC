namespace intranet_mcp_server.Entities
{
    public class VacationBalance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } 
        public int ReferenceYear { get; set; }
        public int EarnedDays { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
