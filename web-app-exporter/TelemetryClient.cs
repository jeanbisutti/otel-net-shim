using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace web_app_exporter;

public class TelemetryClient
{
    private static TracerProvider TracerProvider = null;

    private static LoggerProvider LoggerProvider = null;

    private static MeterProvider MeterProvider = null;

    private static Tracer Tracer = null;

    private static ILogger Logger = null;

    private const string appInsightsTracerName = "ApplicationInsightsTracer";

    public static void init(WebApplicationBuilder builder, string connectionString)

    {
        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracingBuilder =>
                tracingBuilder.AddSource(appInsightsTracerName)
            )
            .UseAzureMonitorExporter(options =>
            {
                options.ConnectionString =
                    connectionString;
            });
    }

    public static void init(WebApplication builder)
    {
        TracerProvider = builder.Services.GetService<TracerProvider>();
        LoggerProvider = builder.Services.GetService<LoggerProvider>();
        MeterProvider = builder.Services.GetService<MeterProvider>();
        Logger = builder.Services.GetService<ILoggerFactory>()?.CreateLogger("TelemetryClientLogger");
        Tracer = TracerProvider.GetTracer(appInsightsTracerName);
    }

    public void TrackDependency(string dependencyName)
    {
        using (Tracer.StartActiveSpan(dependencyName)) ;
    }

    public void TrackEvent(string eventName)
    {
        Logger.LogInformation("{microsoft.custom_event.name}", eventName);
    }

    public void TrackException(Exception exception)
    {
        Logger.LogError(exception, exception.Message);
    }

    public void TrackPageView(string pageViewName)
    {
        // ????
    }

    public void TrackAvaibility()
    {
        // ???
    }

    public void Flush()
    {
        LoggerProvider.ForceFlush();
        TracerProvider.ForceFlush();
        MeterProvider.ForceFlush();
    }
}