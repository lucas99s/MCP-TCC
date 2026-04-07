using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Generic;

/// <summary>
/// Version B - Generic, ambiguous tool descriptions.
/// Delegates all logic to IntranetService (same as Detailed version).
///
/// Tools exposed (4) - mirrors TimesheetToolsDetailed exactly:
///   - get_time_entries
///   - get_hours_summary
///   - get_time_bank_balance
///   - get_employee_availability
/// </summary>
[McpServerToolType]
internal class TimesheetToolsGeneric
{
    private readonly IntranetService _service;
    public TimesheetToolsGeneric(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_time_entries")]
    [Description("Retrieves entries.")]
    public async Task<string> GetTimeEntries(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Optional value.")]
        DateTime? from = null,
        [Description("Optional value.")]
        DateTime? to = null)
    {
        var result = await _service.GetTimeEntriesAsync(employeeId, from, to);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_hours_summary")]
    [Description("Retrieves summary.")]
    public async Task<string> GetHoursSummary(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Optional value.")]
        DateTime? from = null,
        [Description("Optional value.")]
        DateTime? to = null)
    {
        var result = await _service.GetHoursSummaryAsync(employeeId, from, to);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_time_bank_balance")]
    [Description("Retrieves balance.")]
    public async Task<string> GetTimeBankBalance(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Optional value.")]
        int? year = null)
    {
        var result = await _service.GetTimeBankBalanceAsync(employeeId, year);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_employee_availability")]
    [Description("Retrieves availability.")]
    public async Task<string> GetEmployeeAvailability(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Required value.")]
        DateTime date)
    {
        var result = await _service.GetEmployeeAvailabilityAsync(employeeId, date);
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
