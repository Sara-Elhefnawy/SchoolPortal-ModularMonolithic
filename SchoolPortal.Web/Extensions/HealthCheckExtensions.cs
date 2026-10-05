using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SchoolPortal.Web.HealthChecks;

namespace SchoolPortal.Web.Extensions;

public static class HealthCheckExtensions
{
    // WHY a field: the map is the same for all three endpoints, so it is defined once.
    // Unhealthy -> 503 (the host stops routing here), Degraded -> 200 (still serving).
    private static readonly Dictionary<HealthStatus, int> StatusCodes = new()
    {
        [HealthStatus.Healthy] = Microsoft.AspNetCore.Http.StatusCodes.Status200OK,
        [HealthStatus.Degraded] = Microsoft.AspNetCore.Http.StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = Microsoft.AspNetCore.Http.StatusCodes.Status503ServiceUnavailable
    };

    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // "live" = the process is running. It must never depend on external systems,
            // otherwise a DB outage would make the host restart a perfectly fine app.
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            // WHY Brevo stays: BrevoHealthCheck only needs IConfiguration, so it doesn't
            // touch any module's Infrastructure and the arch test doesn't flag it.
            .AddCheck<BrevoHealthCheck>("brevo", tags: ["ready"]);

        // The module checks (students-db, grades-db, ...) are registered by the modules
        // themselves when Program.cs calls AddXModule(...).
        return services;
    }

    public static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        // WHY one helper: three copies of the same options is how the duplicate happened.
        endpoints.MapHealthChecks("/health/live", Options(r => r.Tags.Contains("live")));
        endpoints.MapHealthChecks("/health/ready", Options(r => r.Tags.Contains("ready")));

        // No predicate = run every registered check.
        endpoints.MapHealthChecks("/health", Options(_ => true));

        return endpoints;
    }

    private static HealthCheckOptions Options(Func<HealthCheckRegistration, bool> predicate) => new()
    {
        Predicate = predicate,
        ResponseWriter = HealthCheckResponseWriter.WriteAsync,
        ResultStatusCodes = StatusCodes
    };
}