namespace ExperimentAnalytics.Api.Models;

public sealed class ExperimentDetailDto : ExperimentListItemDto
{
    public string PromptText { get; init; } = string.Empty;
    public string? ExpectedTool { get; init; }
    public string? RawResponse { get; init; }
    public string? Notes { get; init; }
    public string? ResponseId { get; init; }
    public string? ExecutionId { get; init; }
    public string? ModelReturned { get; init; }
    public int? ToolsAvailableCount { get; init; }
    public long? StartedAtMs { get; init; }
    public long? CompletedAt { get; init; }
    public double? TokenEfficiency { get; init; }
    public bool ExpectedToolNotUsed { get; init; }
    public ExperimentBaselineDto? GroupBaseline { get; set; }
}
