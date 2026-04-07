using ExperimentAnalytics.Api.Models;
using ExperimentAnalytics.Api.Services;
using ExperimentAnalytics.Api.Utilities;

namespace ExperimentAnalytics.Api.Endpoints;

public static class ExperimentEndpoints
{
    public static IEndpointRouteBuilder MapExperimentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/experiments").WithTags("Experimentos");

        group.MapGet("/", async (HttpRequest request, ExperimentQueryService service, CancellationToken cancellationToken) =>
        {
            var query = ExperimentFiltersBinder.FromHttpRequest(request);
            return Results.Ok(await service.GetExperimentsAsync(query, cancellationToken));
        });

        group.MapGet("/summary", async (HttpRequest request, ExperimentQueryService service, CancellationToken cancellationToken) =>
        {
            var query = ExperimentFiltersBinder.FromHttpRequest(request);
            return Results.Ok(await service.GetSummaryAsync(query, cancellationToken));
        });

        group.MapGet("/grouped", async (HttpRequest request, string by, ExperimentQueryService service, CancellationToken cancellationToken) =>
        {
            var query = ExperimentFiltersBinder.FromHttpRequest(request);
            return Results.Ok(await service.GetGroupedAsync(by, query, cancellationToken));
        });

        group.MapGet("/filters", async (ExperimentQueryService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetFilterOptionsAsync(cancellationToken)));

        group.MapGet("/insights", async (HttpRequest request, ExperimentQueryService service, CancellationToken cancellationToken) =>
        {
            var query = ExperimentFiltersBinder.FromHttpRequest(request);
            return Results.Ok(await service.GetInsightsAsync(query, cancellationToken));
        });

        group.MapGet("/{id:long}", async (long id, ExperimentQueryService service, CancellationToken cancellationToken) =>
        {
            var result = await service.GetExperimentByIdAsync(id, cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}
