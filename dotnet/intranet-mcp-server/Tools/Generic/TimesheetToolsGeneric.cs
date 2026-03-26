using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Generic;

/// <summary>
/// Version B — Generic, ambiguous tool descriptions.
/// Delegates all logic to IntranetService (same as Detailed version).
///
/// Tools exposed (4) — mirrors TimesheetToolsDetailed exactly:
///   - tool_timesheet_01
///   - tool_timesheet_02
///   - tool_timesheet_03
///   - tool_timesheet_04
/// </summary>
[McpServerToolType]
internal class TimesheetToolsGeneric
{
    private readonly IntranetService _service;
    public TimesheetToolsGeneric(IntranetService service) => _service = service;

    [McpServerTool(Name = "tool_timesheet_01")]
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

    [McpServerTool(Name = "tool_timesheet_02")]
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

    [McpServerTool(Name = "tool_timesheet_03")]
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

    [McpServerTool(Name = "tool_timesheet_04")]
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
