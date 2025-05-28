using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_app_azure_monitor_2.Models;
using web_app_azure_monitor2;

namespace web_app_azure_monitor_2.Controllers;

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
        
        telemetryClient.TrackDependency("My dependency from web controller 2");
        
        telemetryClient.TrackEvent("My event from web controller 2");
        
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
