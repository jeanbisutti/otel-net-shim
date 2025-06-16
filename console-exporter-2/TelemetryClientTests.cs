using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using WireMock.Server;
using Xunit;
using Formatting = Newtonsoft.Json.Formatting;
using Request = WireMock.RequestBuilders.Request;
using Response = WireMock.ResponseBuilders.Response;

namespace console_exporter_2.Tests;

public class TelemetryClientTests : IDisposable
{
    private readonly WireMockServer _mockServer;
    private readonly string _testConnectionString;

    public TelemetryClientTests()
    {
        // Start the mock server on a random available port
        _mockServer = WireMockServer.Start();

        // Create connection string pointing to mock server
        _testConnectionString =
            $"InstrumentationKey=12345678-1234-1234-1234-123456789012;IngestionEndpoint={_mockServer.Url}/v2.1/track;LiveEndpoint={_mockServer.Url}/live";

        // Setup default mock responses
        SetupMockResponses();
    }

    private void SetupMockResponses()
    {
        // Mock the telemetry ingestion endpoint
        _mockServer
            .Given(Request.Create()
                .WithPath("/v2.1/track")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"itemsReceived\": 1, \"itemsAccepted\": 1}"));
    }
    
    [Fact]
    public void TrackEvent_ShouldSendHttpRequest()
    {
        // Arrange
        var client = new TelemetryClient(_testConnectionString);

        // Act
        client.TrackEvent("TestEvent");
        client.Flush(); // Force immediate send

        // Wait a bit for async operations
        Thread.Sleep(1000);

        // Assert
        var requests = _mockServer.LogEntries;
        var telemetryRequests = requests.Where(r => r.RequestMessage.Path.Contains("/v2.1/track"));

        Assert.True(telemetryRequests.Any(), "Expected at least one telemetry request");

        var request = telemetryRequests.First();

        Assert.Equal("POST", request.RequestMessage.Method);
        Assert.Contains("application/json", request.RequestMessage.Headers["Content-Type"].First());
        
        var requestBody = request.RequestMessage.Body;
        VerifyBody(requestBody);
    }

    private void VerifyBody(string current)
    {
        var expectedAsString = ReadFileAsString("response-event.json");
        
        var currentJson = JObject.Parse(current);
        var expectedJSon = JObject.Parse(expectedAsString);

        VerifyTime(currentJson);
        VerifySdkVersion(currentJson);
        
        RemoveNonComparableProperties(currentJson, expectedJSon);

        var message =
            $"Expected: {expectedJSon.ToString(Formatting.Indented)}\nActual: {currentJson.ToString(Formatting.Indented)}";
        Assert.True(JToken.DeepEquals(currentJson, expectedJSon),
            message);
    }

    private static void RemoveNonComparableProperties(JObject currentJson, JObject expectedJSon)
    {
        currentJson.Remove("time");
        expectedJSon.Remove("time");
        if (currentJson["tags"] is JObject actualTags)
        {
            actualTags.Remove("ai.internal.sdkVersion");
        }
        if (expectedJSon["tags"] is JObject expectedTags)
        {
            expectedTags.Remove("ai.internal.sdkVersion");
        }
    }

    private static void VerifySdkVersion(JObject currentJson)
    {
        var tagsToken = currentJson["tags"];
        var sdkVersionToken = tagsToken["ai.internal.sdkVersion"];
        var sdkVersionValue = sdkVersionToken.ToString();
        Assert.False(string.IsNullOrEmpty(sdkVersionValue), "ai.internal.sdkVersion must not be null or empty");
    }

    private static void VerifyTime(JObject currentJson)
    {
        var timeValue = currentJson["time"];
        Assert.False(string.IsNullOrEmpty(timeValue?.ToString()), "Time field must not be null or empty");
    }

    private static string ReadFileAsString(string file, [CallerFilePath] string filePath = "")
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        var fullPath = Path.Join(directoryPath, file);
        return File.ReadAllText(fullPath);
    }
    
    public void Dispose()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
    }
}