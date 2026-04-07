namespace ExperimentAnalytics.Api.Models;

public sealed class GroupedMetricDto
{
    public string GroupKey { get; init; } = string.Empty;
    public int Executions { get; init; }
    public double AvgExecutionTimeMs { get; init; }
    public double MedianExecutionTimeMs { get; init; }
    public double AvgTotalTokens { get; init; }
    public double MedianTotalTokens { get; init; }
    public double AvgReasoningTokens { get; init; }
    public double SuccessRateSelection { get; init; }
    public double SuccessRateResponse { get; init; }
    public double ToolUsageRate { get; init; }
    public double NullSelectedToolRate { get; init; }
    public double AvgToolCallsCount { get; init; }
}
