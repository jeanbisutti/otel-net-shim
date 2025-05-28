using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace console_azure_monitor_2;

public class TelemetryClient
{
    private TracerProvider _tracerProvider;
    private readonly LoggerProvider _loggerProvider;
    private Tracer _tracer;
    private readonly ILogger _logger;

    public TelemetryClient(string connectionString)
    {
        var serviceCollection = new ServiceCollection();

        var appInsightsTracerName = "ApplicationInsightsTracer";

        serviceCollection.AddOpenTelemetry()
            .WithTracing(tracingBuilder => tracingBuilder.AddSource(appInsightsTracerName))
            .UseAzureMonitor(options => options.ConnectionString = connectionString);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        StartHostedServicesAsync(serviceProvider);
        
        _loggerProvider = serviceProvider.GetRequiredService<LoggerProvider>();
        _tracerProvider = serviceProvider.GetRequiredService<TracerProvider>();
        _tracer = _tracerProvider.GetTracer(appInsightsTracerName);
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger("ApplicationInsightsLogger");

    }
    
    private static async Task StartHostedServicesAsync(ServiceProvider serviceProvider)
    {
        var hostedServices = serviceProvider.GetServices<IHostedService>();
        foreach (var hostedService in hostedServices)
        {
            await hostedService.StartAsync(CancellationToken.None);
        }
    }

    public void TrackEvent(string eventName)
    {
        _logger.LogInformation("{microsoft.custom_event.name}", eventName);
    }

    public void TrackDependency(string dependencyName)
    {
        using (_tracer.StartActiveSpan(dependencyName)) ;
    }
    
    public void Flush()
    {
        _loggerProvider.ForceFlush();
        _tracerProvider.ForceFlush();
    }
}