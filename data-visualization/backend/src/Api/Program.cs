using ExperimentAnalytics.Api.Data;
using ExperimentAnalytics.Api.Endpoints;
using ExperimentAnalytics.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSingleton<NpgsqlConnectionFactory>();
builder.Services.AddScoped<ExperimentQueryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "experiment-analytics-api",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapExperimentEndpoints();

app.Run();
