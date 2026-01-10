using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using postgre_mcp_server.Services;

/// <summary>
/// Admin tools for database management.
/// </summary>
internal class AdminTools
{
    private readonly PostgresService _postgresService;

    public AdminTools(PostgresService postgresService)
    {
        _postgresService = postgresService;
    }

    [McpServerTool]
    [Description("Executes a SQL query on the PostgreSQL database and returns the results")]
    public async Task<string> ExecuteSQL([Description("SQL query to execute")] string command)
    {
        try
        {
            // Check if it's a SELECT query
            var trimmedCommand = command.Trim().ToLowerInvariant();
            
            if (trimmedCommand.StartsWith("select"))
            {
                var results = await _postgresService.ExecuteQueryAsync(command);
                return JsonSerializer.Serialize(results, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
            }
            else
            {
                var affectedRows = await _postgresService.ExecuteNonQueryAsync(command);
                return JsonSerializer.Serialize(new 
                { 
                    success = true,
                    affectedRows,
                    message = $"Query executed successfully. {affectedRows} row(s) affected."
                }, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
            }
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
    [Description("Tests the connection to the PostgreSQL database")]
    public async Task<string> TestConnection()
    {
        try
        {
            var isConnected = await _postgresService.TestConnectionAsync();
            return JsonSerializer.Serialize(new 
            { 
                success = isConnected,
                message = isConnected ? "Connection successful!" : "Connection failed."
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