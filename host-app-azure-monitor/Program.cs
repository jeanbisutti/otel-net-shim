using host_app_azure_monitor;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args);

TelemetryClient.init(hostBuilder, "<YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING>");

var host = hostBuilder.Build();

TelemetryClient.init(host);

host.RunAsync();

var telemetryClient = new TelemetryClient();

telemetryClient.TrackDependency("My dependency from host app");

telemetryClient.TrackEvent("My event from host app");

telemetryClient.Flush();
