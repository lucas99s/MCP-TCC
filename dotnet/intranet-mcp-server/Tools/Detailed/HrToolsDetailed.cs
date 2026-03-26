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
///   - tool_hr_01
///   - tool_hr_02
///   - tool_hr_03
///   - tool_hr_04
/// </summary>
[McpServerToolType]
internal class HrToolsDetailed
{
    private readonly IntranetService _service;
    public HrToolsDetailed(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_employees")]
    [Description(
        "Retrieves a list of active employees registered in the corporate HR system. " +
        "Each record includes the employee's full name, corporate email, registration number, " +
        "hire date, employment status, department name, job title, and seniority level. " +
        "Use this tool when the user wants to find employees, look up a person's department or position, " +
        "or explore the organizational structure. " +
        "Optionally filter by department name using a partial case-insensitive match.")]
    public async Task<string> GetEmployees(
        [Description("Optional department name filter (partial match, case-insensitive). Leave empty to return all active employees.")] 
        string? departmentName = null)
    {
        var result = await _service.GetEmployeesAsync(departmentName);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_vacation_balance")]
    [Description(
        "Returns the vacation (annual leave) balance for a specific employee identified by their numeric ID. " +
        "The result includes the reference year, total earned days, days already used, remaining days available, " +
        "and the expiry date of the vacation entitlement. " +
        "Use this tool when the user asks how many vacation days an employee has left, " +
        "when their vacation expires, or to verify their leave entitlement.")]
    public async Task<string> GetVacationBalance(
        [Description("The unique numeric ID of the employee whose vacation balance should be retrieved.")] 
        int employeeId)
    {
        var result = await _service.GetVacationBalanceAsync(employeeId);
        return result is null
            ? Serialize(new { message = "No vacation balance found for this employee." })
            : Serialize(result);
    }

    [McpServerTool(Name = "get_leave_requests")]
    [Description(
        "Returns the leave requests submitted by a specific employee, such as vacation, sick leave, " +
        "justified absence, or day-off requests. " +
        "Each record includes the leave type, start and end dates, current approval status " +
        "(pending, approved, or rejected), and the date the request was submitted. " +
        "Use this tool to check an employee's absence history, track pending approvals, " +
        "or verify whether a specific leave request was approved. " +
        "Optionally filter by status: 'pendente', 'aprovado', or 'rejeitado'.")]
    public async Task<string> GetLeaveRequests(
        [Description("The unique numeric ID of the employee whose leave requests should be retrieved.")] 
        int employeeId,
        [Description("Optional filter by request status. Accepted values: 'pendente', 'aprovado', 'rejeitado'. Leave empty to return all.")] 
        string? status = null)
    {
        var result = await _service.GetLeaveRequestsAsync(employeeId, status);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_departments")]
    [Description(
        "Returns a complete list of all departments in the company, including each department's " +
        "unique ID, name, and description. " +
        "Use this tool when the user wants to explore the organizational structure, " +
        "list all departments, or find the ID of a specific department for use in another query.")]
    public async Task<string> GetDepartments()
    {
        var result = await _service.GetDepartmentsAsync();
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
