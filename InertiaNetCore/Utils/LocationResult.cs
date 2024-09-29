using System.Net;
using InertiaNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Utils;

public class LocationResult : IActionResult
{
    private readonly string _url;

    public LocationResult(string url) => _url = url;

    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context.IsInertiaRequest())
        {
            context.HttpContext.Response.Headers.Append("X-Inertia-Location", _url);
            await new StatusCodeResult((int)HttpStatusCode.Conflict).ExecuteResultAsync(context);
        }

        await new RedirectResult(_url).ExecuteResultAsync(context);
    }
}
