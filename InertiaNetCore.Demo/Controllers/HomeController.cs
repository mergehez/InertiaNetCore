using System.Reflection;
using System.Security.Cryptography;
using InertiaNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Demo.Controllers;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var now = GetNow();
        return Inertia.Render("pages/PageIndex", new InertiaProps
        {
            ["NowDirect"] = now,
            ["NowFunc"] = () => now,
            ["NowAlways"] = Inertia.Always(() => now),
            ["NowLazy"] = Inertia.Lazy(() => now),
            ["NowLazyAsync"] = Inertia.Lazy(async () =>
            {
                await Task.Delay(2000);
                return now;
            }),
            ["Merge"] = Inertia.Merge(() => new[] { RandomNumberGenerator.GetInt32(10) }),
            ["Deferred"] = Inertia.Defer(async () =>
            {
                await Task.Delay(2000);
                return now;
            }),
            ["Deferred1"] = Inertia.Defer(() => "Deferred 1", "group"),
            ["Deferred2"] = Inertia.Defer(() => "Deferred 2", "group"),
            ["Deferred3"] = Inertia.Defer(() => "Deferred 3", "group"),
            ["Deferred4"] = Inertia.Defer(() => "Deferred 4", "group"),
        });
    }

    private static string GetNow()
    {
        return DateTime.UtcNow.ToString("O").Split('T')[1].Replace(".", " ");
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