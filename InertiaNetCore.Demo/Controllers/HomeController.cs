using System.Reflection;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Demo.Controllers;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var now = DateTime.UtcNow.ToString("O").Split('T')[1].Replace(".", " ");
        return Inertia.Render("pages/PageIndex", new InertiaProps
        {
            ["NowDirect"] = now,
            ["NowFunc"] = () => now,
            ["NowAlways"] = Inertia.Always(() => now),
            ["NowLazy"] = Inertia.Lazy(() => now),
        });
    }
    
    [Route("about")]
    public IActionResult About()
    {
        return Inertia.Render("pages/PageAbout", new InertiaProps
        {
            ["Name"] = "InertiaNetCore",
            ["Version"] = Assembly.GetAssembly(typeof(Inertia))?.GetName().Version?.ToString()
        });
    }
}