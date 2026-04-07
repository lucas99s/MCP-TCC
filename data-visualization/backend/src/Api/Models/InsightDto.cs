namespace ExperimentAnalytics.Api.Models;

public sealed class InsightDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Tone { get; init; } = "neutral";
}
