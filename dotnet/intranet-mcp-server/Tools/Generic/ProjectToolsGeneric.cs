using System.ComponentModel;
using System.Text.Json;
using intranet_mcp_server.Services;
using ModelContextProtocol.Server;

namespace intranet_mcp_server.Tools.Generic;

/// <summary>
/// Version B - Generic, ambiguous tool descriptions.
/// Delegates all logic to IntranetService (same as Detailed version).
///
/// Tools exposed (3) - mirrors ProjectToolsDetailed exactly:
///   - get_projects
///   - get_project_allocations
///   - get_project_tasks
/// </summary>
[McpServerToolType]
internal class ProjectToolsGeneric
{
    private readonly IntranetService _service;
    public ProjectToolsGeneric(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_projects")]
    [Description("Retrieves projects.")]
    public async Task<string> GetProjects(
        [Description("Optional value.")]
        string? status = null)
    {
        var result = await _service.GetProjectsAsync(status);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_project_allocations")]
    [Description("Retrieves allocations.")]
    public async Task<string> GetProjectAllocations(
        [Description("Numeric value.")]
        int projectId)
    {
        var result = await _service.GetProjectAllocationsAsync(projectId);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_project_tasks")]
    [Description("Retrieves tasks.")]
    public async Task<string> GetProjectTasks(
        [Description("Numeric value.")]
        int projectId,
        [Description("Optional value.")]
        string? status = null)
    {
        var result = await _service.GetProjectTasksAsync(projectId, status);
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
