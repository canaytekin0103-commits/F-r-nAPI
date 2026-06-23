using FirinApi.Configuration;
using FirinApi.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FirinApi.Extensions;

public static class InfrastructureExtensions
{
    /// <summary>
    /// CORS: Frontend'in (React, Vue vb.) API'ye tarayıcıdan erişmesine izin verir.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>()
            ?? new CorsSettings();

        services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    /// <summary>
    /// Health check: API ve veritabanının ayakta olup olmadığını kontrol eder.
    /// </summary>
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "postgresql"]);

        return services;
    }

    public static WebApplication MapHealthCheckEndpoint(this WebApplication app)
    {
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    }),
                    duration = report.TotalDuration.TotalMilliseconds
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });

        return app;
    }
}
