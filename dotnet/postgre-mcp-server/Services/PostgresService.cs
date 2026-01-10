using Npgsql;
using postgre_mcp_server.Configuration;

namespace postgre_mcp_server.Services;

/// <summary>
/// Service for managing PostgreSQL database connections.
/// </summary>
public class PostgresService
{
    private readonly PostgresConfiguration _config;
    private readonly string _connectionString;

    public PostgresService(PostgresConfiguration config)
    {
        _config = config;
        
        if (!_config.IsValid(out var errorMessage))
        {
            throw new InvalidOperationException($"Invalid PostgreSQL configuration: {errorMessage}");
        }

        _connectionString = _config.GetConnectionString();
    }

    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>
    /// Tests the database connection.
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = await GetConnectionAsync();
            return connection.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Executes a SQL query and returns the results as a list of dictionaries.
    /// </summary>
    public async Task<List<Dictionary<string, object?>>> ExecuteQueryAsync(string sql)
    {
        var results = new List<Dictionary<string, object?>>();

        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }

    /// <summary>
    /// Executes a non-query SQL command (INSERT, UPDATE, DELETE).
    /// </summary>
    public async Task<int> ExecuteNonQueryAsync(string sql)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Gets a list of all tables in the database.
    /// </summary>
    public async Task<List<string>> GetTablesAsync()
    {
        var tables = new List<string>();
        
        var sql = @"
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_type = 'BASE TABLE'
            ORDER BY table_name";

        var results = await ExecuteQueryAsync(sql);
        
        foreach (var row in results)
        {
            if (row.TryGetValue("table_name", out var tableName) && tableName != null)
            {
                tables.Add(tableName.ToString()!);
            }
        }

        return tables;
    }

    /// <summary>
    /// Gets the schema information for a specific table.
    /// </summary>
    public async Task<List<Dictionary<string, object?>>> GetTableSchemaAsync(string tableName)
    {
        var sql = @"
            SELECT 
                column_name,
                data_type,
                character_maximum_length,
                is_nullable,
                column_default
            FROM information_schema.columns
            WHERE table_schema = 'public' 
            AND table_name = @tableName
            ORDER BY ordinal_position";

        await using var connection = await GetConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("tableName", tableName);

        var results = new List<Dictionary<string, object?>>();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }
}
