using Microsoft.AspNetCore.Mvc;

namespace AgendaApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("/")]
    public IActionResult LandingPage()
    {
        return View("LandingPage");
    }

    [HttpGet]
    [Route("/app")]
    public IActionResult App()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    [Route("/dashboard")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
    }
} 