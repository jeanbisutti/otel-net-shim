using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using web_app_azure_monitor.Models;

namespace web_app_azure_monitor.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TracerProvider _tracerProvider;

    public HomeController(ILogger<HomeController> logger, TracerProvider tracerProvider)
    {
        _logger = logger;
        _tracerProvider = tracerProvider;
    }

    public IActionResult Index()
    {
        var telemetryClient = new TelemetryClient();
        
        telemetryClient.TrackDependency("My dependency from web controller");
        
        telemetryClient.TrackEvent("My event from web controller");
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
