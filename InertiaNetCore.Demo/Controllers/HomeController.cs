using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Demo.Controllers;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Inertia.Render("pages/PageIndex", new
        {
            Names = (string[]) ["John", "Doe"],
            Ages = (int[]) [20, 30]
        });
    }
    
    [Route("about")]
    public IActionResult About()
    {
        return Inertia.Render("pages/PageAbout", new
        {
            Name = "InertiaNetCore",
            Version = Assembly.GetAssembly(typeof(Inertia))?.GetName().Version?.ToString()
        });
    }
}