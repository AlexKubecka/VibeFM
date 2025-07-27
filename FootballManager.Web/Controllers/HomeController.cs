using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FootballManager.Web.Models;
using FootballManager.Data;
using FootballManager.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace FootballManager.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HomeController(ILogger<HomeController> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SeedDatabase()
    {
        // Path relative to web project, adjust if needed
        var jsonPath = Path.Combine("..", "FootballManager", "Data", "combined_team_data_wrapped.json");
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FootballManagerDbContext>();
            JsonDbSeeder.SeedFromCombinedTeamData(dbContext, jsonPath);
        }
        TempData["SeedResult"] = "Database seeded from JSON.";
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
