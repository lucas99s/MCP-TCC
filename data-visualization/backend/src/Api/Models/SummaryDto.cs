namespace ExperimentAnalytics.Api.Models;

public sealed class SummaryDto
{
    public int TotalExecutions { get; init; }
    public int TotalCases { get; init; }
    public int TotalModels { get; init; }
    public double AvgExecutionTimeMs { get; init; }
    public double AvgLatencySeconds { get; init; }
    public double AvgTotalTokens { get; init; }
    public double AvgReasoningTokens { get; init; }
    public double SuccessRateSelection { get; init; }
    public double SuccessRateResponse { get; init; }
    public double ToolUsageRate { get; init; }
    public double AvgToolCallsCount { get; init; }
    public double NullSelectedToolRate { get; init; }
    public double FailureRate { get; init; }
    public int RowsWithToolExpectedButNotUsed { get; init; }
}
