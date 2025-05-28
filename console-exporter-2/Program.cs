using console_exporter_2;

var telemetryClient = new TelemetryClient("<YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING>");

telemetryClient.TrackDependency("Dependency to test exporter and service collection");

telemetryClient.TrackEvent("Event to test exporter and service collection");

telemetryClient.Flush();
