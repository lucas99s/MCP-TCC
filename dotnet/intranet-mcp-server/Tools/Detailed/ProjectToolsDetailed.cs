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
///   - tool_projects_01
///   - tool_projects_02
///   - tool_projects_03
/// </summary>
[McpServerToolType]
internal class ProjectToolsDetailed
{
    private readonly IntranetService _service;
    public ProjectToolsDetailed(IntranetService service) => _service = service;

    [McpServerTool(Name = "get_projects")]
    [Description(
        "Retrieves a list of company projects from the project management system. " +
        "Each record includes the project name, unique code, description, start and end dates, " +
        "current status, the responsible department, and the name of the project manager. " +
        "Use this tool when the user wants to find active or past projects, filter projects by status, " +
        "or look up who manages a given project. " +
        "Optionally filter by status (e.g., 'ativo', 'concluido', 'cancelado').")]
    public async Task<string> GetProjects(
        [Description("Optional filter by project status. Examples: 'ativo', 'concluido', 'cancelado'. Leave empty to return all projects.")]
        string? status = null)
    {
        var result = await _service.GetProjectsAsync(status);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_project_allocations")]
    [Description(
        "Returns the list of employees currently or previously allocated to a specific project, " +
        "identified by the project's numeric ID. " +
        "Each record includes the employee's name, their role on the project, and the allocation period. " +
        "Use this tool when the user wants to know who is working on a project, " +
        "who was assigned to a project in a given period, or to view the team composition.")]
    public async Task<string> GetProjectAllocations(
        [Description("The unique numeric ID of the project whose team allocations should be retrieved.")]
        int projectId)
    {
        var result = await _service.GetProjectAllocationsAsync(projectId);
        return Serialize(result);
    }

    [McpServerTool(Name = "get_project_tasks")]
    [Description(
        "Returns the tasks associated with a specific project, identified by the project's numeric ID. " +
        "Each task record includes the title, description, priority, current status, due date, " +
        "and the name of the assigned employee. " +
        "Use this tool to track work items, identify overdue or pending tasks, " +
        "or review what each team member is responsible for in a project. " +
        "Optionally filter by task status (e.g., 'aberto', 'em andamento', 'concluido').")]
    public async Task<string> GetProjectTasks(
        [Description("The unique numeric ID of the project whose tasks should be retrieved.")]
        int projectId,
        [Description("Optional filter by task status. Examples: 'aberto', 'em andamento', 'concluido'. Leave empty to return all tasks.")]
        string? status = null)
    {
        var result = await _service.GetProjectTasksAsync(projectId, status);
        return Serialize(result);
    }

    private static string Serialize(object? data) =>
        JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
