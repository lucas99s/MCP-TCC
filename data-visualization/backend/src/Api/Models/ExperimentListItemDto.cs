namespace ExperimentAnalytics.Api.Models;

public class ExperimentListItemDto
{
    public long Id { get; init; }
    public string CaseId { get; init; } = string.Empty;
    public string? TestRunId { get; init; }
    public string Model { get; init; } = string.Empty;
    public string PromptType { get; init; } = string.Empty;
    public int RunNumber { get; init; }
    public string ToolQuality { get; init; } = string.Empty;
    public string? ExecutionMode { get; init; }
    public string? Status { get; init; }
    public string? ToolChoice { get; init; }
    public string? SelectedTool { get; init; }
    public int? ToolCallsCount { get; init; }
    public bool? SelectionCorrect { get; init; }
    public bool? ResponseCorrect { get; init; }
    public int? TokensInput { get; init; }
    public int? TokensOutput { get; init; }
    public int? TotalTokens { get; init; }
    public int? ReasoningTokens { get; init; }
    public long? ExecutionTimeMs { get; init; }
    public DateTimeOffset? CreatedRecordAt { get; init; }
    public bool ToolUsed { get; init; }
    public double? AvgLatencySeconds { get; init; }
    public double? OutputInputRatio { get; init; }
    public double? ReasoningShare { get; init; }
}
