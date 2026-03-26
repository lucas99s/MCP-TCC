using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace intranet_mcp_server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeBankBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeBankBalances",
                schema: "timesheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    ReferenceMonth = table.Column<int>(type: "integer", nullable: false),
                    ReferenceYear = table.Column<int>(type: "integer", nullable: false),
                    AccumulatedHours = table.Column<double>(type: "double precision", nullable: false),
                    UsedHours = table.Column<double>(type: "double precision", nullable: false),
                    BalanceHours = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeBankBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeBankBalances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeBankBalances_EmployeeId",
                schema: "timesheet",
                table: "TimeBankBalances",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeBankBalances",
                schema: "timesheet");
        }
    }
}
