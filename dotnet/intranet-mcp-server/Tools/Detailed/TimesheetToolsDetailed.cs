using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Detailed;

/// <summary>
/// Version A — Detailed, semantically rich tool descriptions.
/// Delegates all logic to IntranetService.
///
/// Tools exposed (4):
///   - tool_timesheet_01
///   - tool_timesheet_02
///   - tool_timesheet_03
///   - tool_timesheet_04
/// </summary>
[McpServerToolType]
internal class TimesheetToolsDetailed
{
    private readonly IntranetService _service;
    public TimesheetToolsDetailed(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_time_entries")]
    [Description(
        "Returns the individual time entries (work hour records) registered by a specific employee, " +
        "identified by their numeric ID. " +
        "Each record includes the date, number of hours worked, entry type (normal, extra, or justified absence), " +
        "a description of the work performed, and the associated project name. " +
        "Use this tool when the user wants to see the detailed work log of an employee, " +
        "verify hours for a specific date, or audit time records. " +
        "Optionally filter by a date range using ISO 8601 format (yyyy-MM-dd).")]
    public async Task<string> GetTimeEntries(
        [Description("The unique numeric ID of the employee whose time entries should be retrieved.")] 
        int employeeId,
        [Description("Optional start date filter in ISO 8601 format (yyyy-MM-dd). Only entries on or after this date will be returned.")] 
        DateTime? from = null,
        [Description("Optional end date filter in ISO 8601 format (yyyy-MM-dd). Only entries on or before this date will be returned.")] 
        DateTime? to = null)
    {
        var result = await _service.GetTimeEntriesAsync(employeeId, from, to);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_hours_summary")]
    [Description(
        "Returns an aggregated summary of hours worked by a specific employee, grouped by project. " +
        "The result shows the total number of hours attributed to each project within the given period. " +
        "Use this tool when the user wants to know how many hours an employee dedicated to each project, " +
        "generate a workload summary, or analyze effort distribution across projects. " +
        "Optionally filter by a date range using ISO 8601 format (yyyy-MM-dd).")]
    public async Task<string> GetHoursSummary(
        [Description("The unique numeric ID of the employee whose hours summary should be calculated.")] 
        int employeeId,
        [Description("Optional start date filter in ISO 8601 format (yyyy-MM-dd).")] 
        DateTime? from = null,
        [Description("Optional end date filter in ISO 8601 format (yyyy-MM-dd).")] 
        DateTime? to = null)
    {
        var result = await _service.GetHoursSummaryAsync(employeeId, from, to);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_time_bank_balance")]
    [Description(
        "Returns the time bank (overtime hour bank) balance for a specific employee, identified by their numeric ID. " +
        "The time bank records overtime hours that were accumulated and not yet compensated. " +
        "Each record includes the reference month and year, total accumulated overtime hours, " +
        "hours already compensated (used), and the current remaining balance. " +
        "Use this tool when the user wants to check how many overtime hours an employee has accumulated, " +
        "how many hours have been compensated, or what their current time bank balance is. " +
        "Optionally filter by year to narrow down the results.")]
    public async Task<string> GetTimeBankBalance(
        [Description("The unique numeric ID of the employee whose time bank balance should be retrieved.")]
        int employeeId,
        [Description("Optional year filter (e.g., 2024). Leave empty to return balances from all years.")]
        int? year = null)
    {
        var result = await _service.GetTimeBankBalanceAsync(employeeId, year);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_employee_availability")]
    [Description(
        "Checks whether a specific employee is available (i.e., not on approved leave) on a given date. " +
        "The check considers all approved leave requests of any type (vacation, sick leave, day-off, justified absence) " +
        "that overlap with the specified date. It also validates the employee's current employment status. " +
        "Use this tool when scheduling meetings, assigning tasks, or verifying if an employee can be contacted " +
        "or allocated to work on a specific day. " +
        "Returns availability status, the employee's name, and the reason if unavailable.")]
    public async Task<string> GetEmployeeAvailability(
        [Description("The unique numeric ID of the employee to check availability for.")]
        int employeeId,
        [Description("The date to check availability for, in ISO 8601 format (yyyy-MM-dd).")]
        DateTime date)
    {
        var result = await _service.GetEmployeeAvailabilityAsync(employeeId, date);
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
