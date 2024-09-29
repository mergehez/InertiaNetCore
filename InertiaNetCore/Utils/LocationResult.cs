using System.Net;
using InertiaNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Utils;

public class LocationResult(string url) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context.IsInertiaRequest())
        {
            context.HttpContext.Response.Headers.Append("X-Inertia-Location", url);
            await new StatusCodeResult((int)HttpStatusCode.Conflict).ExecuteResultAsync(context);
        }

        await new RedirectResult(url).ExecuteResultAsync(context);
    }
}
