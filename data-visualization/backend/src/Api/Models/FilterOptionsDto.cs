namespace ExperimentAnalytics.Api.Models;

public sealed class FilterOptionsDto
{
    public string[] CaseIds { get; init; } = [];
    public string[] Models { get; init; } = [];
    public string[] PromptTypes { get; init; } = [];
    public string[] ToolQualities { get; init; } = [];
    public string[] Statuses { get; init; } = [];
    public string[] ExecutionModes { get; init; } = [];
}
