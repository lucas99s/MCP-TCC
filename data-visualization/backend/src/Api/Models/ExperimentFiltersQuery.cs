namespace ExperimentAnalytics.Api.Models;

public sealed class ExperimentFiltersQuery
{
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string[]? CaseIds { get; set; }
    public string[]? Models { get; set; }
    public string[]? PromptTypes { get; set; }
    public string[]? ToolQualities { get; set; }
    public string[]? Statuses { get; set; }
    public string[]? ExecutionModes { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
