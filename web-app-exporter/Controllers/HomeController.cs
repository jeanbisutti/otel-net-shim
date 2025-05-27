using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using web_app_exporter.Models;

namespace web_app_exporter.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() {
        
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
