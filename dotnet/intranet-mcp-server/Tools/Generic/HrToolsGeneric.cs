using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Generic;

/// <summary>
/// Version B - Generic, ambiguous tool descriptions.
/// Delegates all logic to IntranetService (same as Detailed version).
///
/// Tools exposed (4) - mirrors HrToolsDetailed exactly:
///   - get_employees
///   - get_vacation_balance
///   - get_leave_requests
///   - get_departments
/// </summary>
[McpServerToolType]
internal class HrToolsGeneric
{
    private readonly IntranetService _service;
    public HrToolsGeneric(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_employees")]
    [Description("Retrieves employees.")]
    public async Task<string> GetEmployees(
        [Description("Optional value.")]
        string? departmentName = null)
    {
        var result = await _service.GetEmployeesAsync(departmentName);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_vacation_balance")]
    [Description("Retrieves balance.")]
    public async Task<string> GetVacationBalance(
        [Description("Numeric value.")]
        int employeeId)
    {
        var result = await _service.GetVacationBalanceAsync(employeeId);
        return result is null
            ? Serialize(new { message = "No vacation balance found for this employee." })
            : Serialize(result);
    }

    [McpServerTool(Name = "get_leave_requests")]
    [Description("Retrieves requests.")]
    public async Task<string> GetLeaveRequests(
        [Description("Numeric value.")]
        int employeeId,
        [Description("Optional value.")]
        string? status = null)
    {
        var result = await _service.GetLeaveRequestsAsync(employeeId, status);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_departments")]
    [Description("Retrieves departments.")]
    public async Task<string> GetDepartments()
    {
        var result = await _service.GetDepartmentsAsync();
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
