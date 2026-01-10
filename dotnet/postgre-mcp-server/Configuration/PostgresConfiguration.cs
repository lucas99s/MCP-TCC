namespace postgre_mcp_server.Configuration;

/// <summary>
/// Configuration class for PostgreSQL connection settings loaded from environment variables.
/// </summary>
public class PostgresConfiguration
{
    /// <summary>
    /// PostgreSQL server host (default: localhost)
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// PostgreSQL server port (default: 5432)
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Database name
    /// </summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// Database username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Database password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Connection timeout in seconds (default: 30)
    /// </summary>
    public int Timeout { get; set; } = 30;

    /// <summary>
    /// Maximum pool size (default: 100)
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Builds the connection string from the configuration properties.
    /// </summary>
    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Timeout={Timeout};Maximum Pool Size={MaxPoolSize}";
    }

    /// <summary>
    /// Validates that required configuration values are present.
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Database))
        {
            errorMessage = "Database name is required. Set POSTGRES_DATABASE environment variable.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            errorMessage = "Username is required. Set POSTGRES_USERNAME environment variable.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            errorMessage = "Password is required. Set POSTGRES_PASSWORD environment variable.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
