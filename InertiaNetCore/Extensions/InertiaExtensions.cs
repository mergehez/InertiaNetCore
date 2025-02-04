using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Extensions;

internal static class InertiaExtensions
{
    private static List<string> GetInertiaHeaderData(this ActionContext context, string header)
    {
        return context.HttpContext.Request.Headers.TryGetValue(header, out var data)
            ? data.FirstOrDefault()?.Split(",")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList() ?? []
            : [];
    }
    internal static List<string> GetPartialData(this ActionContext context)
    {
        return context.GetInertiaHeaderData("X-Inertia-Partial-Data");
    }
    internal static List<string> GetInertiaExcepts(this ActionContext context)
    {
        return context.GetInertiaHeaderData("X-Inertia-Partial-Except");
    }
    internal static List<string> GetInertiaResetData(this ActionContext context)
    {
        return context.GetInertiaHeaderData("X-Inertia-Reset");
    }

    internal static bool IsInertiaPartialComponent(this ActionContext context, string component)
    {
        return context.HttpContext.Request.Headers["X-Inertia-Partial-Component"] == component;
    }

    internal static string RequestedUri(this HttpContext context)
    {
        return Uri.UnescapeDataString(context.Request.GetEncodedPathAndQuery());
    }
}

public static class InertiaPublicExtensions
{
    public static bool IsInertiaRequest(this HttpContext context)
    {
        return bool.TryParse(context.Request.Headers["X-Inertia"], out _);
    }
}
