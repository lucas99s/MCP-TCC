using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Generic;

/// <summary>
/// Version B — Generic, ambiguous tool descriptions.
// Delegates all logic to IntranetService (same as Detailed version).
//
/// Tools exposed (3) — mirrors PayrollToolsDetailed exactly:
///   - tool_payroll_01
///   - tool_payroll_02
///   - tool_payroll_03
/// </summary>
[McpServerToolType]
internal class PayrollToolsGeneric
{
    private readonly IntranetService _service;
    public PayrollToolsGeneric(IntranetService service) => _service = service;

    [McpServerTool(Name = "tool_payroll_01")]
    [Description("Retrieves summary.")]
    public async Task<string> GetPayrollSummaries(
        [Description("Numeric value.")] 
        int employeeId,
        [Description("Optional value.")] 
        int? year = null)
    {
        var result = await _service.GetPayrollSummariesAsync(employeeId, year);
        return Serialize(result);
    }

    [McpServerTool(Name = "tool_payroll_02")]
    [Description("Retrieves annual data.")]
    public async Task<string> GetPayrollAnnualSummary(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Optional value.")]
        int? fromYear = null,
        [Description("Optional value.")]
        int? toYear = null)
    {
        var result = await _service.GetPayrollAnnualSummaryAsync(employeeId, fromYear, toYear);
        return Serialize(result);
    }

    [McpServerTool(Name = "tool_payroll_03")]
    [Description("Retrieves ratio.")]
    public async Task<string> GetPayrollDeductionRatio(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Numeric value.")]
        int month,
        [Description("Numeric value.")]
        int year)
    {
        var result = await _service.GetPayrollDeductionRatioAsync(employeeId, month, year);
        return result is null
            ? Serialize(new { message = "No payroll record found for the specified employee, month, and year." })
            : Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
