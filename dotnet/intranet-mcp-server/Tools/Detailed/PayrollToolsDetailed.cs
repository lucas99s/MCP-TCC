using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Detailed;

/// <summary>
/// Version A — Detailed, semantically rich tool descriptions.
/// Delegates all logic to IntranetService.
///
/// Tools exposed (3):
///   - tool_payroll_01
///   - tool_payroll_02
///   - tool_payroll_03
/// </summary>
[McpServerToolType]
internal class PayrollToolsDetailed
{
    private readonly IntranetService _service;
    public PayrollToolsDetailed(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_payroll_summaries")]
    [Description(
        "Returns the payroll (salary) summaries for a specific employee, identified by their numeric ID. " +
        "Each record includes the reference month and year, gross salary, total deductions, net salary, " +
        "payment date, and payment status (pending or paid). " +
        "Use this tool when the user wants to consult salary history, verify a specific month's net pay, " +
        "check if a payroll was processed, or review deductions for a given period. " +
        "Optionally filter by year to narrow down the results.")]
    public async Task<string> GetPayrollSummaries(
        [Description("The unique numeric ID of the employee whose payroll summaries should be retrieved.")] 
        int employeeId,
        [Description("Optional year filter (e.g., 2024). Leave empty to return summaries from all years.")] 
        int? year = null)
    {
        var result = await _service.GetPayrollSummariesAsync(employeeId, year);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_payroll_annual_summary")]
    [Description(
        "Returns an annual aggregation of payroll data for a specific employee, grouped by year. " +
        "Each record shows the total gross salary, total deductions, total net salary, and the number " +
        "of months processed in that year. " +
        "Use this tool when the user wants to compare earnings across years, calculate annual income, " +
        "verify how much was deducted in a given year, or generate a multi-year financial overview. " +
        "Optionally filter with a year range (fromYear / toYear) to narrow down the results.")]
    public async Task<string> GetPayrollAnnualSummary(
        [Description("The unique numeric ID of the employee whose annual payroll summary should be retrieved.")]
        int employeeId,
        [Description("Optional lower bound year filter (inclusive). Leave empty to include all past years.")]
        int? fromYear = null,
        [Description("Optional upper bound year filter (inclusive). Leave empty to include up to the most recent year.")]
        int? toYear = null)
    {
        var result = await _service.GetPayrollAnnualSummaryAsync(employeeId, fromYear, toYear);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_payroll_deduction_ratio")]
    [Description(
        "Returns a detailed breakdown of salary deductions for a specific employee in a given month and year. " +
        "The result includes the gross salary, total deductions, net salary, payment date, status, " +
        "and the effective deduction rate expressed as a percentage of the gross salary. " +
        "Use this tool when the user wants to understand how much of their gross pay was deducted, " +
        "compare the deduction burden between months, verify a specific month's payslip composition, " +
        "or audit tax and benefit withholdings.")]
    public async Task<string> GetPayrollDeductionRatio(
        [Description("The unique numeric ID of the employee whose deduction ratio should be analysed.")]
        int employeeId,
        [Description("The reference month as a number from 1 (January) to 12 (December).")]
        int month,
        [Description("The reference year (e.g., 2024).")]
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
