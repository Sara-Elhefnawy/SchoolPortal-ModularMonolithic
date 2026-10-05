using Microsoft.AspNetCore.Antiforgery;

namespace SchoolPortal.Web.Extensions;

public static class AntiforgeryExtensions
{
    public static IServiceCollection AddAppAntiforgery(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            // JavaScript sends the request token in this header.
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "SchoolPortal.Csrf";

            // WHY HttpOnly: only the server compares this cookie. JS gets its token
            // from /api/antiforgery/token instead.
            options.Cookie.HttpOnly = true;

            // WHY SameAsRequest: works over HTTP locally and HTTPS in production.
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });

        return services;
    }

    // WHY validate in our own middleware: an unhandled AntiforgeryValidationException becomes
    // a 500 page, and the frontend needs a JSON "code" to know it may retry with a fresh token.
    public static IApplicationBuilder UseCsrfValidation(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var isStateChanging = HttpMethods.IsPost(context.Request.Method) ||
                                  HttpMethods.IsPut(context.Request.Method) ||
                                  HttpMethods.IsPatch(context.Request.Method) ||
                                  HttpMethods.IsDelete(context.Request.Method);

            if (isStateChanging)
            {
                var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

                try
                {
                    await antiforgery.ValidateRequestAsync(context);
                }
                catch (AntiforgeryValidationException ex)
                {
                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("CsrfValidation");

                    logger.LogWarning(ex, "CSRF rejected on {Path}. Authenticated={Auth}", context.Request.Path,
                        context.User.Identity?.IsAuthenticated);

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        code = "csrf", message = "Security token was rejected. Please try again."
                    });
                    return;
                }
            }

            await next();
        });
    }

    // WHY an endpoint instead of a cookie: the token is created for whoever the user is
    // RIGHT NOW, so it can't go stale after login/logout. Safe because no CORS is configured,
    // so other websites can't read this response. Don't add permissive CORS later.
    public static IEndpointRouteBuilder MapAntiforgeryTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/antiforgery/token", (HttpContext context, IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            return Results.Ok(new { token = tokens.RequestToken });
        }).AllowAnonymous().DisableAntiforgery();

        return endpoints;
    }
}