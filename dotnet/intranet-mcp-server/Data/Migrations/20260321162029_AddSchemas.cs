using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace intranet_mcp_server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.EnsureSchema(
                name: "projects");

            migrationBuilder.EnsureSchema(
                name: "timesheet");

            migrationBuilder.RenameTable(
                name: "VacationBalances",
                newName: "VacationBalances",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "TimeEntries",
                newName: "TimeEntries",
                newSchema: "timesheet");

            migrationBuilder.RenameTable(
                name: "ProjectTasks",
                newName: "ProjectTasks",
                newSchema: "projects");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "projects");

            migrationBuilder.RenameTable(
                name: "ProjectAllocations",
                newName: "ProjectAllocations",
                newSchema: "projects");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "Positions",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "PayrollSummaries",
                newName: "PayrollSummaries",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                newName: "LeaveRequests",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employees",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Departments",
                newSchema: "hr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "VacationBalances",
                schema: "hr",
                newName: "VacationBalances");

            migrationBuilder.RenameTable(
                name: "TimeEntries",
                schema: "timesheet",
                newName: "TimeEntries");

            migrationBuilder.RenameTable(
                name: "ProjectTasks",
                schema: "projects",
                newName: "ProjectTasks");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "projects",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "ProjectAllocations",
                schema: "projects",
                newName: "ProjectAllocations");

            migrationBuilder.RenameTable(
                name: "Positions",
                schema: "hr",
                newName: "Positions");

            migrationBuilder.RenameTable(
                name: "PayrollSummaries",
                schema: "finance",
                newName: "PayrollSummaries");

            migrationBuilder.RenameTable(
                name: "LeaveRequests",
                schema: "hr",
                newName: "LeaveRequests");

            migrationBuilder.RenameTable(
                name: "Employees",
                schema: "hr",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "Departments",
                schema: "hr",
                newName: "Departments");
        }
    }
}
