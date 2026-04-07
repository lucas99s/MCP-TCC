using System.Text;
using Dapper;
using ExperimentAnalytics.Api.Models;

namespace ExperimentAnalytics.Api.Utilities;

public static class SqlFilterBuilder
{
    public static (string whereClause, DynamicParameters parameters) Build(ExperimentFiltersQuery query)
    {
        var where = new StringBuilder(" WHERE 1=1 ");
        var parameters = new DynamicParameters();

        if (query.StartDate.HasValue)
        {
            where.Append(" AND created_record_at >= @StartDate ");
            parameters.Add("StartDate", query.StartDate.Value.UtcDateTime);
        }

        if (query.EndDate.HasValue)
        {
            where.Append(" AND created_record_at <= @EndDate ");
            parameters.Add("EndDate", query.EndDate.Value.UtcDateTime);
        }

        AddArrayFilter(where, parameters, "case_id", "CaseIds", query.CaseIds);
        AddArrayFilter(where, parameters, "model", "Models", query.Models);
        AddArrayFilter(where, parameters, "prompt_type", "PromptTypes", query.PromptTypes);
        AddArrayFilter(where, parameters, "tool_quality", "ToolQualities", query.ToolQualities);
        AddArrayFilter(where, parameters, "status", "Statuses", query.Statuses);
        AddArrayFilter(where, parameters, "execution_mode", "ExecutionModes", query.ExecutionModes);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            where.Append("""
                 AND (
                    case_id ILIKE @Search
                    OR COALESCE(test_run_id, '') ILIKE @Search
                    OR model ILIKE @Search
                    OR prompt_text ILIKE @Search
                    OR COALESCE(selected_tool, '') ILIKE @Search
                 )
                """);
            parameters.Add("Search", $"%{query.Search.Trim()}%");
        }

        return (where.ToString(), parameters);
    }

    private static void AddArrayFilter(StringBuilder where, DynamicParameters parameters, string columnName, string parameterName, string[]? values)
    {
        if (values is null || values.Length == 0)
            return;

        where.Append($" AND {columnName} = ANY(@{parameterName}) ");
        parameters.Add(parameterName, values);
    }
}
