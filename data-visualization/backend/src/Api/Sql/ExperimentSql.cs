namespace ExperimentAnalytics.Api.Sql;

public static class ExperimentSql
{
    public const string BaseProjection = """
        SELECT
            id,
            case_id AS CaseId,
            test_run_id AS TestRunId,
            model AS Model,
            prompt_type AS PromptType,
            run_number AS RunNumber,
            tool_quality AS ToolQuality,
            execution_mode AS ExecutionMode,
            status AS Status,
            tool_choice AS ToolChoice,
            selected_tool AS SelectedTool,
            tool_calls_count AS ToolCallsCount,
            selection_correct AS SelectionCorrect,
            response_correct AS ResponseCorrect,
            tokens_input AS TokensInput,
            tokens_output AS TokensOutput,
            total_tokens AS TotalTokens,
            reasoning_tokens AS ReasoningTokens,
            execution_time_ms AS ExecutionTimeMs,
            created_record_at AS CreatedRecordAt,
            (
                COALESCE(tool_calls_count, 0) > 0
                OR selected_tool IS NOT NULL
            ) AS ToolUsed,
            ROUND(COALESCE(execution_time_ms, 0) / 1000.0, 3) AS AvgLatencySeconds,
            ROUND(COALESCE(tokens_output::numeric / NULLIF(tokens_input, 0), 0), 4) AS OutputInputRatio,
            ROUND(COALESCE(reasoning_tokens::numeric / NULLIF(total_tokens, 0), 0), 4) AS ReasoningShare
        FROM mcp_experiment_results
        """;
}
