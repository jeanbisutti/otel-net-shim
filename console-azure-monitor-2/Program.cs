using console_azure_monitor_2;

class Program
{
    static void Main(string[] args)
    {
        var telemetryClient = new TelemetryClient("<YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING>");

        telemetryClient.TrackDependency("Dependency to test Azure Monitor and service collection");

        telemetryClient.TrackEvent("Event to test Azure Monitor and service collection");

        telemetryClient.Flush();
    }

}