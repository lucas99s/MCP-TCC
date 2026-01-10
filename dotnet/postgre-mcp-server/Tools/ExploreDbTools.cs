using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using postgre_mcp_server.Services;

internal class ExploreDbTools
{
    private readonly PostgresService _postgresService;

    public ExploreDbTools(PostgresService postgresService)
    {
        _postgresService = postgresService;
    }

    [McpServerTool]
    [Description("Explores the database schema and returns a list of tables")]
    public async Task<string> ListTables()
    {
        try
        {
            var tables = await _postgresService.GetTablesAsync();
            return JsonSerializer.Serialize(new
            {
                success = true,
                count = tables.Count,
                tables
            }, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }

    [McpServerTool]
    [Description("Gets the schema information for a specific table including columns, data types, and constraints")]
    public async Task<string> GetTableSchema([Description("Name of the table to describe")] string tableName)
    {
        try
        {
            var schema = await _postgresService.GetTableSchemaAsync(tableName);
            return JsonSerializer.Serialize(new
            {
                success = true,
                tableName,
                columns = schema
            }, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}