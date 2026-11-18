using System.Globalization;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Naheulbook.Web.Telemetry;

/// <summary>
/// OpenTelemetry setup (traces, logs and metrics) exported with OTLP. Traces and metrics use the OpenTelemetry SDK,
/// logs are sent directly from Serilog with Serilog.Sinks.OpenTelemetry (so the messages are rendered without the quotes
/// Serilog adds around string values, and the message template / properties are kept as attributes).
/// Everything is configured with the standard OTEL_* variables, read from the configuration (so either environment
/// variables or root keys of an appsettings file):
/// <list type="bullet">
/// <item>OTEL_EXPORTER_OTLP_ENDPOINT, OTEL_EXPORTER_OTLP_PROTOCOL, OTEL_EXPORTER_OTLP_HEADERS (and the per-signal
/// variants): handled by the OTLP exporter. Default: http://localhost:4317 using gRPC.</item>
/// <item>OTEL_SDK_DISABLED=true: disables everything.</item>
/// <item>OTEL_TRACES_EXPORTER / OTEL_LOGS_EXPORTER / OTEL_METRICS_EXPORTER=none: disables one signal.</item>
/// <item>OTEL_SERVICE_NAME, OTEL_RESOURCE_ATTRIBUTES: resource attributes (e.g. deployment.environment=staging).</item>
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

    /// <summary>
    /// Send Serilog events to the OTLP endpoint. Uses the same OTEL_* configuration as the OpenTelemetry SDK, read from
    /// <paramref name="configuration"/> (so it works with appsettings files and not only with environment variables).
    /// </summary>
    public static LoggerConfiguration WriteToNaheulbookOpenTelemetry(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        if (IsTrue(configuration["OTEL_SDK_DISABLED"]) || !IsSignalEnabled(configuration, "OTEL_LOGS_EXPORTER"))
            return loggerConfiguration;

        return loggerConfiguration.WriteTo.OpenTelemetry(options =>
            {
                var protocol = configuration["OTEL_EXPORTER_OTLP_LOGS_PROTOCOL"] ?? configuration["OTEL_EXPORTER_OTLP_PROTOCOL"];
                options.Protocol = protocol?.Trim().ToLowerInvariant() switch
                {
                    null or "" or "grpc" => OtlpProtocol.Grpc,
                    "http/protobuf" => OtlpProtocol.HttpProtobuf,
                    _ => throw new InvalidOperationException($"Unsupported OTLP protocol '{protocol}', expected grpc or http/protobuf"),
                };

                // Base endpoint: the sink appends /v1/logs for http/protobuf. The per-signal endpoint is used as is.
                var logsEndpoint = configuration["OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"];
                if (!string.IsNullOrWhiteSpace(logsEndpoint))
                    options.LogsEndpoint = logsEndpoint.Trim();
                else
                    options.Endpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]?.Trim() is { Length: > 0 } endpoint
                        ? endpoint
                        : options.Protocol == OtlpProtocol.Grpc ? "http://localhost:4317" : "http://localhost:4318";

                foreach (var (key, value) in ParseKeyValueList(configuration["OTEL_EXPORTER_OTLP_HEADERS"]))
                    options.Headers[key] = value;
                foreach (var (key, value) in ParseKeyValueList(configuration["OTEL_EXPORTER_OTLP_LOGS_HEADERS"]))
                    options.Headers[key] = value;

                foreach (var (key, value) in ParseKeyValueList(configuration["OTEL_RESOURCE_ATTRIBUTES"]))
                    options.ResourceAttributes[key] = value;
                options.ResourceAttributes["service.name"] = configuration["OTEL_SERVICE_NAME"]?.Trim() is { Length: > 0 } serviceName
                    ? serviceName
                    : ServiceName;

                // Do not trace the HTTP calls made to export the logs
                options.OnBeginSuppressInstrumentation = SuppressInstrumentationScope.Begin;
            },
            ignoreEnvironment: true
        );
    }

    /// <summary>
    /// Parse the W3C Baggage like format used by OTEL_EXPORTER_OTLP_HEADERS and OTEL_RESOURCE_ATTRIBUTES: key1=value1,key2=value2
    /// </summary>
    private static IEnumerable<(string Key, string Value)> ParseKeyValueList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        foreach (var entry in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separatorIndex = entry.IndexOf('=');
            if (separatorIndex <= 0)
                throw new InvalidOperationException($"Invalid OTEL key/value entry '{entry}', expected key=value");
            yield return (
                Uri.UnescapeDataString(entry[..separatorIndex].Trim()),
                Uri.UnescapeDataString(entry[(separatorIndex + 1)..].Trim())
            );
        }
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
