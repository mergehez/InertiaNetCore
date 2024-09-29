using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Extensions;

internal static class InertiaExtensions
{
    internal static IEnumerable<string> Only(this object obj, IEnumerable<string> only) =>
        obj.GetType().GetProperties().Select(c => c.Name)
            .Intersect(only, StringComparer.OrdinalIgnoreCase).ToList();

    internal static List<string> GetPartialData(this ActionContext context) =>
        context.HttpContext.Request.Headers["X-Inertia-Partial-Data"]
            .FirstOrDefault()?.Split(",")
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList() ?? new List<string>();

    internal static bool IsInertiaPartialComponent(this ActionContext context, string component) =>
        context.HttpContext.Request.Headers["X-Inertia-Partial-Component"] == component;

    internal static string RequestedUri(this HttpContext context) =>
        Uri.UnescapeDataString(context.Request.GetEncodedPathAndQuery());

    internal static string RequestedUri(this ActionContext context) => context.HttpContext.RequestedUri();

    internal static bool IsInertiaRequest(this HttpContext context) =>
        bool.TryParse(context.Request.Headers["X-Inertia"], out _);

    internal static bool IsInertiaRequest(this ActionContext context) => context.HttpContext.IsInertiaRequest();

    internal static string ToCamelCase(this string s) => JsonNamingPolicy.CamelCase.ConvertName(s);
}
