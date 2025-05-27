using otel_sdk;

var AzureCloudRoleName = "MyRoleName";

var telemetryClient = new TelemetryClient(AzureCloudRoleName, "<YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING>");

telemetryClient.TrackTrace("Log from OTel SDK");

telemetryClient.TrackRequest("http://example.com", "GET", "200");

telemetryClient.TrackDependency("MyDependency");

telemetryClient.TrackEvent("MyEvent");

var exception = new Exception("MyException");
telemetryClient.TrackException(exception);

telemetryClient.TrackPageView("myPageView");

telemetryClient.Flush();