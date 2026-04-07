namespace ExperimentAnalytics.Api.Models;

public sealed class ExperimentBaselineDto
{
    public string GroupLabel { get; init; } = string.Empty;
    public double AvgExecutionTimeMs { get; init; }
    public double AvgTotalTokens { get; init; }
    public double AvgReasoningTokens { get; init; }
    public double SuccessRateSelection { get; init; }
    public double SuccessRateResponse { get; init; }
}
