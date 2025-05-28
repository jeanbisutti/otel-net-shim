using System.Diagnostics;
using console_exporter_2;
using Microsoft.AspNetCore.Mvc;
using web_app_exporter_2.Models;

namespace web_app_exporter_2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var telemetryClient = new TelemetryClient("<YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING>");
        
        telemetryClient.TrackDependency("My dependency from exproter 2");
        
        telemetryClient.TrackEvent("My event from exporter 2");
        
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
