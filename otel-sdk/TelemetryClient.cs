using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace otel_sdk;

public class TelemetryClient
{
    private readonly OpenTelemetrySdk _otelSdk;
    private readonly ILogger<TelemetryClient> _logger;
    private readonly Tracer _tracer;

    public TelemetryClient(string azureCloudRoleName, string connectionString)
    {
        var appInsightsTracerName = "ApplicationInsightsTracer";

        _otelSdk = OpenTelemetrySdk.Create(builder =>
        {
            builder.ConfigureResource(resource => resource.AddService(azureCloudRoleName));

            builder.WithLogging(loggingBuilder =>
            {
                loggingBuilder.AddAzureMonitorLogExporter(options =>
                    options.ConnectionString =
                        connectionString);
            });
            
            builder.WithTracing(tracingBuilder =>
            {
                tracingBuilder.AddSource(appInsightsTracerName);
                tracingBuilder.AddAzureMonitorTraceExporter(options => options.ConnectionString =
                    connectionString);
            });
        });


        _tracer = _otelSdk.TracerProvider.GetTracer(appInsightsTracerName);
        _logger = _otelSdk.GetLoggerFactory().CreateLogger<TelemetryClient>();
    }
    
    public void TrackTrace(string traceName)
    {
        _logger.LogInformation(traceName);
    }

    public void TrackRequest(string url, string httpMethod, string status)
    {
        var spanName = httpMethod; // should be {http-method} + " " + {relative-url}
        using (var activity = _tracer.StartActiveSpan(spanName, SpanKind.Server))
        {
            activity.SetAttribute("http.url", url);
            activity.SetAttribute("http.method", httpMethod);
        }
    }

    public void TrackDependency(string dependencyName)
    {
        using (_tracer.StartActiveSpan(dependencyName)) ;
    }

    public void TrackEvent(string eventName)
    {
        _logger.LogInformation("{microsoft.custom_event.name}", eventName);
    }

    public void TrackException(Exception exception)
    {
        _logger.LogError(exception, exception.Message);
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
        _otelSdk.LoggerProvider.ForceFlush();
        _otelSdk.TracerProvider.ForceFlush();
        _otelSdk.MeterProvider.ForceFlush();
    }
}