using System.Runtime.CompilerServices;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Html;

[assembly: InternalsVisibleTo("InertiaCoreTests")]

namespace InertiaNetCore;

public static class Inertia
{
    private static ResponseFactory _factory = default!;

    internal static void UseFactory(ResponseFactory factory) => _factory = factory;

    public static Response Render(string component, InertiaProps? props = default) => _factory.Render(component, props);
    
    public static Response Render(string component, Dictionary<string, object?>? props = default) => _factory.Render(component, InertiaProps.Create(props));

    public static Task<IHtmlContent> Head(dynamic model) => _factory.Head(model);

    public static Task<IHtmlContent> Html(dynamic model) => _factory.Html(model);

    public static void SetVersion(object? version) => _factory.SetVersion(version);

    public static string? GetVersion() => _factory.GetVersion();

    public static LocationResult Location(string url) => _factory.Location(url);

    public static void Share(string key, object? value) => _factory.Share(key, value);

    public static void Share(InertiaProps data) => _factory.Share(data);

    public static LazyProp<T> Lazy<T>(Func<T?> callback) => _factory.Lazy(callback);
    public static LazyProp<T> Lazy<T>(Func<Task<T?>> callback) => _factory.Lazy(callback);
    
    public static AlwaysProp<T> Always<T>(Func<T?> callback) => _factory.Always(callback);
    public static AlwaysProp<T> Always<T>(Func<Task<T?>> callback) => _factory.Always(callback);
}
