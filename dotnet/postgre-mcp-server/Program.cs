using postgre_mcp_server.Configuration;
using postgre_mcp_server.Services;

// Load .env file in development if present
try
{
    DotNetEnv.Env.Load();
}
catch
{
    // ignore if .env not present or load fails
}

var builder = WebApplication.CreateBuilder(args);

// Load PostgreSQL configuration from environment variables
var postgresConfig = new PostgresConfiguration
{
    Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
    Port = int.TryParse(Environment.GetEnvironmentVariable("POSTGRES_PORT"), out var port) ? port : 5432,
    Database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE") ?? string.Empty,
    Username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? string.Empty,
    Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty,
    Timeout = int.TryParse(Environment.GetEnvironmentVariable("POSTGRES_TIMEOUT"), out var timeout) ? timeout : 30,
    MaxPoolSize = int.TryParse(Environment.GetEnvironmentVariable("POSTGRES_MAX_POOL_SIZE"), out var maxPool) ? maxPool : 100
};

// Validate configuration
if (!postgresConfig.IsValid(out var errorMessage))
{
    Console.Error.WriteLine($"Configuration Error: {errorMessage}");
    Console.Error.WriteLine("\nRequired environment variables:");
    Console.Error.WriteLine("  - POSTGRES_DATABASE: Database name");
    Console.Error.WriteLine("  - POSTGRES_USERNAME: Database username");
    Console.Error.WriteLine("  - POSTGRES_PASSWORD: Database password");
    Console.Error.WriteLine("\nOptional environment variables:");
    Console.Error.WriteLine("  - POSTGRES_HOST: Database host (default: localhost)");
    Console.Error.WriteLine("  - POSTGRES_PORT: Database port (default: 5432)");
    Console.Error.WriteLine("  - POSTGRES_TIMEOUT: Connection timeout in seconds (default: 30)");
    Console.Error.WriteLine("  - POSTGRES_MAX_POOL_SIZE: Maximum pool size (default: 100)");
    Environment.Exit(1);
}

// Register PostgreSQL service as singleton
var postgresService = new PostgresService(postgresConfig);
builder.Services.AddSingleton(postgresService);

// Test connection at startup
try
{
    var isConnected = await postgresService.TestConnectionAsync();
    if (!isConnected)
    {
        Console.Error.WriteLine("Failed to connect to PostgreSQL database. Please check your configuration.");
        Environment.Exit(1);
    }
    Console.WriteLine($"Successfully connected to PostgreSQL database: {postgresConfig.Database}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Database connection error: {ex.Message}");
    Environment.Exit(1);
}

// Add the MCP services: the transport to use (http) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<AdminTools>()
    .WithTools<ExploreDbTools>();

var app = builder.Build();
app.MapMcp();
//app.UseHttpsRedirection();

app.Run();
