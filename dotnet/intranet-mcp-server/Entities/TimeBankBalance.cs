namespace intranet_mcp_server.Entities;

public class TimeBankBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ReferenceMonth { get; set; }  // 1-12
    public int ReferenceYear { get; set; }
    public double AccumulatedHours { get; set; }  // horas extras acumuladas
    public double UsedHours { get; set; }          // horas compensadas
    public double BalanceHours { get; set; }       // saldo atual
    public DateTime LastUpdated { get; set; }

    // Navigation property
    public Employee? Employee { get; set; }
}
