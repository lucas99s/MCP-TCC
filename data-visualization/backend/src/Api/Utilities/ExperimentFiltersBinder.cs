using ExperimentAnalytics.Api.Models;

namespace ExperimentAnalytics.Api.Utilities;

public static class ExperimentFiltersBinder
{
    public static ExperimentFiltersQuery FromHttpRequest(HttpRequest request)
    {
        var query = request.Query;

        return new ExperimentFiltersQuery
        {
            StartDate = ParseDateTimeOffset(query["startDate"]),
            EndDate = ParseDateTimeOffset(query["endDate"]),
            CaseIds = ParseArray(query["caseIds"]),
            Models = ParseArray(query["models"]),
            PromptTypes = ParseArray(query["promptTypes"]),
            ToolQualities = ParseArray(query["toolQualities"]),
            Statuses = ParseArray(query["statuses"]),
            ExecutionModes = ParseArray(query["executionModes"]),
            Page = ParseInt(query["page"]) ?? 1,
            PageSize = ParseInt(query["pageSize"]) ?? 20,
            Search = ParseString(query["search"]),
            SortBy = ParseString(query["sortBy"]),
            SortDirection = ParseString(query["sortDirection"])
        };
    }

    private static string[]? ParseArray(Microsoft.Extensions.Primitives.StringValues values)
    {
        if (values.Count == 0)
            return null;

        var items = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .SelectMany(value => value!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return items.Length == 0 ? null : items;
    }

    private static DateTimeOffset? ParseDateTimeOffset(string? value) =>
        DateTimeOffset.TryParse(value, out var parsed) ? parsed : null;

    private static int? ParseInt(string? value) =>
        int.TryParse(value, out var parsed) ? parsed : null;

    private static string? ParseString(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
