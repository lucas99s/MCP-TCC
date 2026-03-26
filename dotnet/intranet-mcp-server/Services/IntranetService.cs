using intranet_mcp_server.Data;
using intranet_mcp_server.Entities;
using Microsoft.EntityFrameworkCore;

namespace intranet_mcp_server.Services;

/// <summary>
/// Shared service that contains all business logic and database queries
/// used by both Detailed and Generic MCP tool variants.
/// This ensures zero code duplication between tool description versions.
/// </summary>
public class IntranetService
{
    private readonly AppDbContext _db;

    public IntranetService(AppDbContext db)
    {
        _db = db;
    }

    // ── HR ───────────────────────────────────────────────────

    /// <summary>
    /// Returns all active employees, optionally filtered by department name.
    /// </summary>
    public async Task<List<object>> GetEmployeesAsync(string? departmentName = null)
    {
        var query = _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Status == "ativo");

        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(e => e.Department!.Name.ToLower().Contains(departmentName.ToLower()));

        return await query.Select(e => (object)new
        {
            e.Id,
            e.FullName,
            e.Email,
            e.RegistrationNumber,
            e.HireDate,
            e.Status,
            Department = e.Department!.Name,
            Position   = e.Position!.Title,
            Level      = e.Position!.Level
        }).ToListAsync();
    }

    /// <summary>
    /// Returns the vacation balance for a specific employee by their ID.
    /// </summary>
    public async Task<object?> GetVacationBalanceAsync(int employeeId)
    {
        return await _db.VacationBalances
            .Where(v => v.EmployeeId == employeeId)
            .OrderByDescending(v => v.ReferenceYear)
            .Select(v => (object)new
            {
                v.EmployeeId,
                v.ReferenceYear,
                v.EarnedDays,
                v.UsedDays,
                v.RemainingDays,
                v.ExpiryDate
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns leave requests for a specific employee, optionally filtered by status.
    /// </summary>
    public async Task<List<object>> GetLeaveRequestsAsync(int employeeId, string? status = null)
    {
        var query = _db.LeaveRequests.Where(lr => lr.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(lr => lr.Status.ToLower() == status.ToLower());

        return await query
            .OrderByDescending(lr => lr.RequestedAt)
            .Select(lr => (object)new
            {
                lr.Id,
                lr.Type,
                lr.StartDate,
                lr.EndDate,
                lr.Status,
                lr.RequestedAt
            }).ToListAsync();
    }

    /// <summary>
    /// Returns all departments with their names and descriptions.
    /// </summary>
    public async Task<List<object>> GetDepartmentsAsync()
    {
        return await _db.Departments
            .Select(d => (object)new
            {
                d.Id,
                d.Name,
                d.Description
            }).ToListAsync();
    }

    // ── PROJECTS ───────────────────────────────────────────────────

    /// <summary>
    /// Returns all projects, optionally filtered by status.
    /// </summary>
    public async Task<List<object>> GetProjectsAsync(string? status = null)
    {
        var query = _db.Projects
            .Include(p => p.Department)
            .Include(p => p.ProjectManager)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status.ToLower() == status.ToLower());

        return await query.Select(p => (object)new
        {
            p.Id,
            p.Name,
            p.Code,
            p.Description,
            p.StartDate,
            p.EndDate,
            p.Status,
            Department     = p.Department!.Name,
            ProjectManager = p.ProjectManager!.FullName
        }).ToListAsync();
    }

    /// <summary>
    /// Returns all employees allocated to a specific project by project ID.
    /// </summary>
    public async Task<List<object>> GetProjectAllocationsAsync(int projectId)
    {
        return await _db.ProjectAllocations
            .Include(pa => pa.Employee)
            .Where(pa => pa.ProjectId == projectId)
            .Select(pa => (object)new
            {
                pa.EmployeeId,
                Employee      = pa.Employee!.FullName,
                pa.RoleOnProject,
                pa.StartDate,
                pa.EndDate
            }).ToListAsync();
    }

    /// <summary>
    /// Returns tasks for a specific project, optionally filtered by status.
    /// </summary>
    public async Task<List<object>> GetProjectTasksAsync(int projectId, string? status = null)
    {
        var query = _db.ProjectTasks
            .Include(pt => pt.AssignedEmployee)
            .Where(pt => pt.ProjectId == projectId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(pt => pt.Status.ToLower() == status.ToLower());

        return await query.Select(pt => (object)new
        {
            pt.Id,
            pt.Title,
            pt.Description,
            pt.Status,
            pt.Priority,
            pt.DueDate,
            AssignedEmployee = pt.AssignedEmployee!.FullName
        }).ToListAsync();
    }

    // ── TIMESHEET ───────────────────────────────────────────────────

    /// <summary>
    /// Returns time entries for a specific employee within an optional date range.
    /// </summary>
    public async Task<List<object>> GetTimeEntriesAsync(int employeeId, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.TimeEntries
            .Include(te => te.Project)
            .Where(te => te.EmployeeId == employeeId);

        if (from.HasValue)
            query = query.Where(te => te.EntryDate >= from.Value);
        if (to.HasValue)
            query = query.Where(te => te.EntryDate <= to.Value);

        return await query
            .OrderByDescending(te => te.EntryDate)
            .Select(te => (object)new
            {
                te.Id,
                te.EntryDate,
                te.HoursWorked,
                te.EntryType,
                te.Description,
                Project = te.Project!.Name
            }).ToListAsync();
    }

    /// <summary>
    /// Returns the total hours worked by an employee grouped by project within an optional date range.
    /// </summary>
    public async Task<List<object>> GetHoursSummaryAsync(int employeeId, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.TimeEntries
            .Include(te => te.Project)
            .Where(te => te.EmployeeId == employeeId);

        if (from.HasValue)
            query = query.Where(te => te.EntryDate >= from.Value);
        if (to.HasValue)
            query = query.Where(te => te.EntryDate <= to.Value);

        return await query
            .GroupBy(te => new { te.ProjectId, te.Project!.Name })
            .Select(g => (object)new
            {
                ProjectId   = g.Key.ProjectId,
                ProjectName = g.Key.Name,
                TotalHours  = g.Sum(te => te.HoursWorked)
            }).ToListAsync();
    }

    /// <summary>
    /// Returns the time bank (overtime) balance for a specific employee,
    /// optionally filtered by year.
    /// </summary>
    public async Task<List<object>> GetTimeBankBalanceAsync(int employeeId, int? year = null)
    {
        var query = _db.TimeBankBalances.Where(tb => tb.EmployeeId == employeeId);

        if (year.HasValue)
            query = query.Where(tb => tb.ReferenceYear == year.Value);

        return await query
            .OrderByDescending(tb => tb.ReferenceYear)
            .ThenByDescending(tb => tb.ReferenceMonth)
            .Select(tb => (object)new
            {
                tb.ReferenceMonth,
                tb.ReferenceYear,
                tb.AccumulatedHours,
                tb.UsedHours,
                tb.BalanceHours,
                tb.LastUpdated
            }).ToListAsync();
    }

    /// <summary>
    /// Returns the availability status of an employee on a given date,
    /// by checking approved leave requests that overlap with the date.
    /// </summary>
    public async Task<object> GetEmployeeAvailabilityAsync(int employeeId, DateTime date)
    {
        var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

        var employee = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.Id == employeeId)
            .Select(e => new { e.Id, e.FullName, e.Status, Department = e.Department!.Name })
            .FirstOrDefaultAsync();

        if (employee is null)
            return new { available = false, reason = "Employee not found." };

        if (employee.Status != "ativo")
            return new
            {
                employeeId,
                employeeName = employee.FullName,
                date         = utcDate,
                available    = false,
                reason       = $"Employee status is '{employee.Status}'."
            };

        var activeLeave = await _db.LeaveRequests
            .Where(lr =>
                lr.EmployeeId == employeeId &&
                lr.Status     == "aprovado" &&
                lr.StartDate  <= utcDate    &&
                lr.EndDate    >= utcDate)
            .Select(lr => new { lr.Type, lr.StartDate, lr.EndDate })
            .FirstOrDefaultAsync();

        if (activeLeave is not null)
            return new
            {
                employeeId,
                employeeName = employee.FullName,
                date         = utcDate,
                available    = false,
                reason       = $"Employee has an approved leave of type '{activeLeave.Type}' from {activeLeave.StartDate:yyyy-MM-dd} to {activeLeave.EndDate:yyyy-MM-dd}."
            };

        return new
        {
            employeeId,
            employeeName = employee.FullName,
            date         = utcDate,
            available    = true,
            reason       = "No approved leave found for this date."
        };
    }

    // ── Payroll ───────────────────────────────────────────────────

    /// <summary>
    /// Returns payroll summaries for a specific employee, optionally filtered by year.
    /// </summary>
    public async Task<List<object>> GetPayrollSummariesAsync(int employeeId, int? year = null)
    {
        var query = _db.PayrollSummaries.Where(ps => ps.EmployeeId == employeeId);

        if (year.HasValue)
            query = query.Where(ps => ps.ReferenceYear == year.Value);

        return await query
            .OrderByDescending(ps => ps.ReferenceYear)
            .ThenByDescending(ps => ps.ReferenceMonth)
            .Select(ps => (object)new
            {
                ps.ReferenceMonth,
                ps.ReferenceYear,
                ps.GrossSalary,
                ps.Deductions,
                ps.NetSalary,
                ps.PaymentDate,
                ps.Status
            }).ToListAsync();
    }


    /// <summary>
    /// Returns an annual aggregation of payroll data for a specific employee:
    /// total gross salary, total deductions, and total net salary per year.
    /// Optionally filtered to a specific year range.
    /// </summary>
    public async Task<List<object>> GetPayrollAnnualSummaryAsync(int employeeId, int? fromYear = null, int? toYear = null)
    {
        var query = _db.PayrollSummaries.Where(ps => ps.EmployeeId == employeeId);

        if (fromYear.HasValue)
            query = query.Where(ps => ps.ReferenceYear >= fromYear.Value);
        if (toYear.HasValue)
            query = query.Where(ps => ps.ReferenceYear <= toYear.Value);

        return await query
            .GroupBy(ps => ps.ReferenceYear)
            .OrderByDescending(g => g.Key)
            .Select(g => (object)new
            {
                ReferenceYear    = g.Key,
                TotalGrossSalary = g.Sum(ps => ps.GrossSalary),
                TotalDeductions  = g.Sum(ps => ps.Deductions),
                TotalNetSalary   = g.Sum(ps => ps.NetSalary),
                MonthsProcessed  = g.Count()
            }).ToListAsync();
    }

    /// <summary>
    /// Returns the deduction ratio analysis for a specific employee in a given month and year.
    /// Includes gross salary, total deductions, net salary, and the effective deduction rate as a percentage.
    /// </summary>
    public async Task<object?> GetPayrollDeductionRatioAsync(int employeeId, int month, int year)
    {
        var record = await _db.PayrollSummaries
            .Where(ps => ps.EmployeeId == employeeId
                      && ps.ReferenceMonth == month
                      && ps.ReferenceYear  == year)
            .Select(ps => new
            {
                ps.ReferenceMonth,
                ps.ReferenceYear,
                ps.GrossSalary,
                ps.Deductions,
                ps.NetSalary,
                ps.PaymentDate,
                ps.Status
            })
            .FirstOrDefaultAsync();

        if (record is null)
            return null;

        var deductionRate = record.GrossSalary > 0
            ? Math.Round((double)(record.Deductions / record.GrossSalary) * 100, 2)
            : 0.0;

        return new
        {
            record.ReferenceMonth,
            record.ReferenceYear,
            record.GrossSalary,
            record.Deductions,
            record.NetSalary,
            record.PaymentDate,
            record.Status,
            DeductionRatePercent = deductionRate
        };
    }
}
