using intranet_mcp_server.Data;
using intranet_mcp_server.Services;
using intranet_mcp_server.Tools.Detailed;
using intranet_mcp_server.Tools.Generic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework Core com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Shared service — single implementation used by both tool versions
builder.Services.AddScoped<IntranetService>();

// Tool description version: "Detailed" (Version A) or "Generic" (Version B)
// Controlled via appsettings.json: "ToolVersion": "Detailed" | "Generic"
var toolVersion = builder.Configuration["ToolVersion"] ?? "Detailed";

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("Startup");
logger.LogInformation("Tool version selected: {ToolVersion} ({Label})",
    toolVersion,
    toolVersion == "Generic" ? "Version B" : "Version A");

var mcpBuilder = builder.Services
    .AddMcpServer()
    .WithHttpTransport();

if (toolVersion == "Generic")
{
    mcpBuilder
        .WithTools<HrToolsGeneric>()
        .WithTools<ProjectToolsGeneric>()
        .WithTools<TimesheetToolsGeneric>()
        .WithTools<PayrollToolsGeneric>();
}
else
{
    mcpBuilder
        .WithTools<HrToolsDetailed>()
        .WithTools<ProjectToolsDetailed>()
        .WithTools<TimesheetToolsDetailed>()
        .WithTools<PayrollToolsDetailed>();
}

var app = builder.Build();

// Seed database with sample data on startup (skips if already seeded)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

app.MapMcp();
app.UseHttpsRedirection();

app.Run();
