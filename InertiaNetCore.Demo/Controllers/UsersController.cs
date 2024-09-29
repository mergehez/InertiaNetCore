using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Demo.Controllers;

[Route("users")]
public class UsersController : Controller
{
    public IActionResult Index()
    {
        return Inertia.Render("pages/PageUsers", new
        {
            Users = (string[]) ["First User", "Second User"],
        });
    }
}