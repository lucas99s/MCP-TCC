using Dapper;
using ExperimentAnalytics.Api.Data;
using ExperimentAnalytics.Api.Models;
using ExperimentAnalytics.Api.Sql;
using ExperimentAnalytics.Api.Utilities;

namespace ExperimentAnalytics.Api.Services;

public sealed class ExperimentQueryService
{
    private static readonly HashSet<string> AllowedGroupBy = ["prompt_type", "model", "tool_quality", "execution_mode", "status", "case_id"];
    private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["created_record_at"] = "created_record_at",
        ["execution_time_ms"] = "execution_time_ms",
        ["total_tokens"] = "total_tokens",
        ["reasoning_tokens"] = "reasoning_tokens",
        ["tokens_input"] = "tokens_input",
        ["tokens_output"] = "tokens_output",
        ["run_number"] = "run_number",
        ["case_id"] = "case_id",
        ["model"] = "model",
        ["prompt_type"] = "prompt_type",
        ["status"] = "status"
    };

    private readonly NpgsqlConnectionFactory _connectionFactory;

    public ExperimentQueryService(NpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SummaryDto> GetSummaryAsync(ExperimentFiltersQuery query, CancellationToken cancellationToken)
    {
        var (where, parameters) = SqlFilterBuilder.Build(query);
        var sql = $"""
            SELECT
                COUNT(*)::int AS TotalExecutions,
                COUNT(DISTINCT case_id)::int AS TotalCases,
                COUNT(DISTINCT model)::int AS TotalModels,
                ROUND(COALESCE(AVG(execution_time_ms), 0), 2) AS AvgExecutionTimeMs,
                ROUND(COALESCE(AVG(execution_time_ms) / 1000.0, 0), 3) AS AvgLatencySeconds,
                ROUND(COALESCE(AVG(total_tokens), 0), 2) AS AvgTotalTokens,
                ROUND(COALESCE(AVG(reasoning_tokens), 0), 2) AS AvgReasoningTokens,
                ROUND(COALESCE(AVG(CASE WHEN selection_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateSelection,
                ROUND(COALESCE(AVG(CASE WHEN response_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateResponse,
                ROUND(COALESCE(AVG(CASE WHEN COALESCE(tool_calls_count, 0) > 0 OR selected_tool IS NOT NULL THEN 1.0 ELSE 0.0 END), 0), 4) AS ToolUsageRate,
                ROUND(COALESCE(AVG(COALESCE(tool_calls_count, 0)), 0), 2) AS AvgToolCallsCount,
                ROUND(COALESCE(AVG(CASE WHEN selected_tool IS NULL THEN 1.0 ELSE 0.0 END), 0), 4) AS NullSelectedToolRate,
                ROUND(COALESCE(AVG(CASE WHEN response_correct IS TRUE THEN 0.0 ELSE 1.0 END), 0), 4) AS FailureRate,
                COUNT(*) FILTER (WHERE expected_tool IS NOT NULL AND selected_tool IS NULL)::int AS RowsWithToolExpectedButNotUsed
            FROM mcp_experiment_results
            {where}
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleAsync<SummaryDto>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<PagedResult<ExperimentListItemDto>> GetExperimentsAsync(ExperimentFiltersQuery query, CancellationToken cancellationToken)
    {
        var (where, parameters) = SqlFilterBuilder.Build(query);
        var sortColumn = !string.IsNullOrWhiteSpace(query.SortBy) && AllowedSortColumns.TryGetValue(query.SortBy, out var mappedSortColumn)
            ? mappedSortColumn
            : "created_record_at";
        var sortDirection = string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        parameters.Add("Offset", (page - 1) * pageSize);
        parameters.Add("Limit", pageSize);

        var countSql = $"SELECT COUNT(*) FROM mcp_experiment_results {where};";
        var dataSql = $"""
            {ExperimentSql.BaseProjection}
            {where}
            ORDER BY {sortColumn} {sortDirection}, id DESC
            OFFSET @Offset
            LIMIT @Limit
            """;

        using var connection = _connectionFactory.Create();
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var items = (await connection.QueryAsync<ExperimentListItemDto>(new CommandDefinition(dataSql, parameters, cancellationToken: cancellationToken))).ToList();

        return new PagedResult<ExperimentListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ExperimentDetailDto?> GetExperimentByIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id,
                case_id AS CaseId,
                test_run_id AS TestRunId,
                model AS Model,
                prompt_type AS PromptType,
                prompt_text AS PromptText,
                run_number AS RunNumber,
                tool_quality AS ToolQuality,
                expected_tool::text AS ExpectedTool,
                execution_id AS ExecutionId,
                execution_mode AS ExecutionMode,
                started_at_ms AS StartedAtMs,
                response_id AS ResponseId,
                status AS Status,
                model_returned AS ModelReturned,
                completed_at AS CompletedAt,
                execution_time_ms AS ExecutionTimeMs,
                raw_response AS RawResponse,
                tokens_input AS TokensInput,
                tokens_output AS TokensOutput,
                total_tokens AS TotalTokens,
                reasoning_tokens AS ReasoningTokens,
                tool_choice AS ToolChoice,
                tools_available_count AS ToolsAvailableCount,
                selected_tool AS SelectedTool,
                tool_calls_count AS ToolCallsCount,
                selection_correct AS SelectionCorrect,
                response_correct AS ResponseCorrect,
                notes AS Notes,
                created_record_at AS CreatedRecordAt,
                (COALESCE(tool_calls_count, 0) > 0 OR selected_tool IS NOT NULL) AS ToolUsed,
                ROUND(COALESCE(execution_time_ms, 0) / 1000.0, 3) AS AvgLatencySeconds,
                ROUND(COALESCE(tokens_output::numeric / NULLIF(tokens_input, 0), 0), 4) AS OutputInputRatio,
                ROUND(COALESCE(reasoning_tokens::numeric / NULLIF(total_tokens, 0), 0), 4) AS ReasoningShare,
                CASE WHEN response_correct IS TRUE THEN ROUND(1000.0 / NULLIF(total_tokens, 0), 4) ELSE 0 END AS TokenEfficiency,
                (expected_tool IS NOT NULL AND selected_tool IS NULL) AS ExpectedToolNotUsed
            FROM mcp_experiment_results
            WHERE id = @Id;
            """;

        using var connection = _connectionFactory.Create();
        var experiment = await connection.QuerySingleOrDefaultAsync<ExperimentDetailDto>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        if (experiment is null)
            return null;

        const string baselineSql = """
            SELECT
                CONCAT(case_id, ' / ', model, ' / ', prompt_type, ' / ', tool_quality) AS GroupLabel,
                ROUND(COALESCE(AVG(execution_time_ms), 0), 2) AS AvgExecutionTimeMs,
                ROUND(COALESCE(AVG(total_tokens), 0), 2) AS AvgTotalTokens,
                ROUND(COALESCE(AVG(reasoning_tokens), 0), 2) AS AvgReasoningTokens,
                ROUND(COALESCE(AVG(CASE WHEN selection_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateSelection,
                ROUND(COALESCE(AVG(CASE WHEN response_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateResponse
            FROM mcp_experiment_results
            WHERE case_id = @CaseId AND model = @Model AND prompt_type = @PromptType AND tool_quality = @ToolQuality
            GROUP BY case_id, model, prompt_type, tool_quality;
            """;

        experiment.GroupBaseline = await connection.QuerySingleOrDefaultAsync<ExperimentBaselineDto>(
            new CommandDefinition(
                baselineSql,
                new { experiment.CaseId, experiment.Model, experiment.PromptType, experiment.ToolQuality },
                cancellationToken: cancellationToken));

        return experiment;
    }

    public async Task<IReadOnlyList<GroupedMetricDto>> GetGroupedAsync(string by, ExperimentFiltersQuery query, CancellationToken cancellationToken)
    {
        if (!AllowedGroupBy.Contains(by))
            throw new ArgumentException($"Agrupamento '{by}' não suportado.");

        var (where, parameters) = SqlFilterBuilder.Build(query);
        var sql = $"""
            SELECT
                COALESCE({by}::text, 'N/A') AS GroupKey,
                COUNT(*)::int AS Executions,
                ROUND(COALESCE(AVG(execution_time_ms), 0), 2) AS AvgExecutionTimeMs,
                ROUND(COALESCE((PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY execution_time_ms))::numeric, 0), 2) AS MedianExecutionTimeMs,
                ROUND(COALESCE(AVG(total_tokens), 0), 2) AS AvgTotalTokens,
                ROUND(COALESCE((PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY total_tokens))::numeric, 0), 2) AS MedianTotalTokens,
                ROUND(COALESCE(AVG(reasoning_tokens), 0), 2) AS AvgReasoningTokens,
                ROUND(COALESCE(AVG(CASE WHEN selection_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateSelection,
                ROUND(COALESCE(AVG(CASE WHEN response_correct IS TRUE THEN 1.0 ELSE 0.0 END), 0), 4) AS SuccessRateResponse,
                ROUND(COALESCE(AVG(CASE WHEN COALESCE(tool_calls_count, 0) > 0 OR selected_tool IS NOT NULL THEN 1.0 ELSE 0.0 END), 0), 4) AS ToolUsageRate,
                ROUND(COALESCE(AVG(CASE WHEN selected_tool IS NULL THEN 1.0 ELSE 0.0 END), 0), 4) AS NullSelectedToolRate,
                ROUND(COALESCE(AVG(COALESCE(tool_calls_count, 0)), 0), 2) AS AvgToolCallsCount
            FROM mcp_experiment_results
            {where}
            GROUP BY {by}
            ORDER BY Executions DESC, GroupKey;
            """;

        using var connection = _connectionFactory.Create();
        var rows = await connection.QueryAsync<GroupedMetricDto>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return rows.ToList();
    }

    public async Task<FilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ARRAY(SELECT DISTINCT case_id FROM mcp_experiment_results WHERE case_id IS NOT NULL ORDER BY case_id) AS CaseIds,
                ARRAY(SELECT DISTINCT model FROM mcp_experiment_results WHERE model IS NOT NULL ORDER BY model) AS Models,
                ARRAY(SELECT DISTINCT prompt_type FROM mcp_experiment_results WHERE prompt_type IS NOT NULL ORDER BY prompt_type) AS PromptTypes,
                ARRAY(SELECT DISTINCT tool_quality FROM mcp_experiment_results WHERE tool_quality IS NOT NULL ORDER BY tool_quality) AS ToolQualities,
                ARRAY(SELECT DISTINCT status FROM mcp_experiment_results WHERE status IS NOT NULL ORDER BY status) AS Statuses,
                ARRAY(SELECT DISTINCT execution_mode FROM mcp_experiment_results WHERE execution_mode IS NOT NULL ORDER BY execution_mode) AS ExecutionModes;
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleAsync<FilterOptionsDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<InsightDto>> GetInsightsAsync(ExperimentFiltersQuery query, CancellationToken cancellationToken)
    {
        var groupedByPrompt = await GetGroupedAsync("prompt_type", query, cancellationToken);
        var groupedByToolQuality = await GetGroupedAsync("tool_quality", query, cancellationToken);
        var summary = await GetSummaryAsync(query, cancellationToken);
        var insights = new List<InsightDto>();

        var ambiguous = groupedByPrompt.FirstOrDefault(x => x.GroupKey.Equals("ambiguous", StringComparison.OrdinalIgnoreCase));
        var clear = groupedByPrompt.FirstOrDefault(x => x.GroupKey.Equals("clear", StringComparison.OrdinalIgnoreCase));
        var detailed = groupedByToolQuality.FirstOrDefault(x => x.GroupKey.Equals("detailed", StringComparison.OrdinalIgnoreCase));
        var generic = groupedByToolQuality.FirstOrDefault(x => x.GroupKey.Equals("generic", StringComparison.OrdinalIgnoreCase));

        if (ambiguous is not null && clear is not null && clear.AvgTotalTokens > 0)
        {
            var delta = ((ambiguous.AvgTotalTokens - clear.AvgTotalTokens) / clear.AvgTotalTokens) * 100;
            insights.Add(new InsightDto
            {
                Id = "prompt-token-delta",
                Title = "Diferença de custo entre prompts",
                Description = $"Prompts ambíguos consumiram em média {delta:F1}% mais tokens do que prompts claros no filtro atual.",
                Tone = delta > 0 ? "warning" : "positive"
            });
        }

        if (detailed is not null && generic is not null)
        {
            var responseDelta = (detailed.SuccessRateResponse - generic.SuccessRateResponse) * 100;
            var selectionDelta = (detailed.SuccessRateSelection - generic.SuccessRateSelection) * 100;
            var tokenDelta = generic.AvgTotalTokens > 0
                ? ((detailed.AvgTotalTokens - generic.AvgTotalTokens) / generic.AvgTotalTokens) * 100
                : 0;

            insights.Add(new InsightDto
            {
                Id = "tool-quality-response-delta",
                Title = responseDelta >= 0
                    ? "Descrição detailed elevou a resposta correta"
                    : "Descrição generic respondeu melhor no recorte atual",
                Description =
                    $"No filtro atual, a diferença entre detailed e generic foi de {responseDelta:F1} p.p. em resposta correta e {selectionDelta:F1} p.p. em acerto de seleção.",
                Tone = responseDelta >= 0 ? "positive" : "warning"
            });

            insights.Add(new InsightDto
            {
                Id = "tool-quality-cost-delta",
                Title = tokenDelta >= 0
                    ? "Detailed aumentou o custo médio"
                    : "Detailed reduziu o custo médio",
                Description =
                    $"Comparando a descrição das tools, detailed ficou {Math.Abs(tokenDelta):F1}% {(tokenDelta >= 0 ? "acima" : "abaixo")} de generic em tokens médios.",
                Tone = tokenDelta >= 0 ? "warning" : "positive"
            });
        }

        if (summary.ToolUsageRate == 0)
        {
            insights.Add(new InsightDto
            {
                Id = "no-tool-usage",
                Title = "Nenhuma execução utilizou tools",
                Description = "No recorte atual, nenhuma execução acionou tools. Isso sugere falha de seleção, baixa clareza semântica ou fluxo mal instrumentado.",
                Tone = "warning"
            });
        }

        if (summary.SuccessRateResponse == 0)
        {
            insights.Add(new InsightDto
            {
                Id = "zero-response-success",
                Title = "Taxa de resposta correta em 0%",
                Description = "A taxa de resposta correta está em 0% no filtro atual. Vale cruzar prompt_type, model e tool_quality para localizar a regressão.",
                Tone = "critical"
            });
        }

        return insights;
    }
}
