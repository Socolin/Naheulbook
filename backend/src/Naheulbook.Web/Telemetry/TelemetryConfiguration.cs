using System.Globalization;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Naheulbook.Web.Telemetry;

/// <summary>
/// OpenTelemetry setup (traces, logs and metrics) exported with the standard OTLP exporter.
/// Everything is configured with the standard OTEL_* variables, read from the configuration (so either environment
/// variables or root keys of an appsettings file):
/// <list type="bullet">
/// <item>OTEL_EXPORTER_OTLP_ENDPOINT, OTEL_EXPORTER_OTLP_PROTOCOL, OTEL_EXPORTER_OTLP_HEADERS (and the per-signal
/// variants): handled by the OTLP exporter. Default: http://localhost:4317 using gRPC.</item>
/// <item>OTEL_SDK_DISABLED=true: disables everything.</item>
/// <item>OTEL_TRACES_EXPORTER / OTEL_LOGS_EXPORTER / OTEL_METRICS_EXPORTER=none: disables one signal.</item>
/// <item>OTEL_TRACES_SAMPLER / OTEL_TRACES_SAMPLER_ARG: always_on, always_off, traceidratio, parentbased_always_on,
/// parentbased_always_off, parentbased_traceidratio. Default: parentbased_always_on.</item>
/// </list>
/// </summary>
public static class TelemetryConfiguration
{
    public const string ServiceName = "Naheulbook";

    public static IServiceCollection AddNaheulbookTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        if (IsTrue(configuration["OTEL_SDK_DISABLED"]))
            return services;

        var builder = services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(ServiceName));

        if (IsSignalEnabled(configuration, "OTEL_TRACES_EXPORTER"))
        {
            builder.WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(o => o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health"))
                    .AddHttpClientInstrumentation();

                var sampler = CreateSampler(configuration);
                if (sampler != null)
                    tracing.SetSampler(sampler);
            });
        }

        if (IsSignalEnabled(configuration, "OTEL_LOGS_EXPORTER"))
        {
            services.Configure<OpenTelemetryLoggerOptions>(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
            });
            builder.WithLogging();
        }

        if (IsSignalEnabled(configuration, "OTEL_METRICS_EXPORTER"))
        {
            builder.WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("System.Runtime"));
        }

        builder.UseOtlpExporter();

        return services;
    }

    private static bool IsTrue(string? value)
    {
        return string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSignalEnabled(IConfiguration configuration, string key)
    {
        return !string.Equals(configuration[key]?.Trim(), "none", StringComparison.OrdinalIgnoreCase);
    }

    private static Sampler? CreateSampler(IConfiguration configuration)
    {
        var name = configuration["OTEL_TRACES_SAMPLER"]?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(name))
            return null;

        var ratio = 1.0;
        var arg = configuration["OTEL_TRACES_SAMPLER_ARG"];
        if (!string.IsNullOrWhiteSpace(arg))
        {
            if (!double.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out ratio) || ratio is < 0 or > 1)
                throw new InvalidOperationException($"Invalid OTEL_TRACES_SAMPLER_ARG '{arg}', expected a number between 0 and 1");
        }

        return name switch
        {
            "always_on" => new AlwaysOnSampler(),
            "always_off" => new AlwaysOffSampler(),
            "traceidratio" => new TraceIdRatioBasedSampler(ratio),
            "parentbased_always_on" => new ParentBasedSampler(new AlwaysOnSampler()),
            "parentbased_always_off" => new ParentBasedSampler(new AlwaysOffSampler()),
            "parentbased_traceidratio" => new ParentBasedSampler(new TraceIdRatioBasedSampler(ratio)),
            _ => throw new InvalidOperationException($"Unsupported OTEL_TRACES_SAMPLER '{name}'"),
        };
    }
}
