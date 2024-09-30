using System.Net;
using InertiaNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Utils;

/// <summary>
/// see doc: https://inertiajs.com/redirects#external-redirects
/// </summary>
public class LocationResult(string url) : IActionResult
{
    /// <inheritdoc />
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context.HttpContext.IsInertiaRequest())
        {
            context.HttpContext.Response.Headers.Append("X-Inertia-Location", url);
            await new StatusCodeResult((int)HttpStatusCode.Conflict).ExecuteResultAsync(context);
        }

        await new RedirectResult(url).ExecuteResultAsync(context);
    }
}
