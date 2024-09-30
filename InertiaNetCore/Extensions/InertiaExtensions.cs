using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Extensions;

internal static class InertiaExtensions
{
    internal static List<string> GetPartialData(this ActionContext context)
    {
        return context.HttpContext.Request.Headers.TryGetValue("X-Inertia-Partial-Data", out var data)
            ? data.FirstOrDefault()?.Split(",")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList() ?? []
            : [];
    }

    internal static bool IsInertiaPartialComponent(this ActionContext context, string component)
    {
        return context.HttpContext.Request.Headers["X-Inertia-Partial-Component"] == component;
    }

    internal static string RequestedUri(this HttpContext context)
    {
        return Uri.UnescapeDataString(context.Request.GetEncodedPathAndQuery());
    }

    internal static bool IsInertiaRequest(this HttpContext context)
    {
        return bool.TryParse(context.Request.Headers["X-Inertia"], out _);
    }
}
