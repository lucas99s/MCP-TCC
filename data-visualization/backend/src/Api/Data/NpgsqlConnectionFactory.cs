using System.Data;
using Npgsql;

namespace ExperimentAnalytics.Api.Data;

public sealed class NpgsqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection Create()
    {
        var connectionString = _configuration.GetConnectionString("Experiments")
            ?? throw new InvalidOperationException("ConnectionStrings:Experiments não configurada.");

        return new NpgsqlConnection(connectionString);
    }
}
