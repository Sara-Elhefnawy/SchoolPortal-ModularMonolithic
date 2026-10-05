using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolPortal.Web.HealthChecks;

public static class HealthCheckResponseWriter
{
    // WHY static and reused: JsonSerializerOptions caches type metadata,
    // so creating a new one per request is wasteful.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        // WHY: prints "Healthy" instead of the enum number 2.
        Converters = { new JsonStringEnumConverter() }
    };

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var payload = new
        {
            status = report.Status,
            totalDurationMs = (int)report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status,
                description = e.Value.Description,
                durationMs = (int)e.Value.Duration.TotalMilliseconds,

                // WHY only the exception TYPE: this endpoint is public. A full message
                // can leak connection strings or hostnames. Full details belong in logs.
                error = e.Value.Exception?.GetType().Name
            })
        };

        // WriteAsJsonAsync sets Content-Type: application/json for us.
        return context.Response.WriteAsJsonAsync(payload, JsonOptions);
    }
}