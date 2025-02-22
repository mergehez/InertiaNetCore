﻿using System.Runtime.CompilerServices;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Html;

[assembly: InternalsVisibleTo("InertiaCoreTests")]

namespace InertiaNetCore;

public static class Inertia
{
    private static ResponseFactory _factory = default!;

    internal static void UseFactory(ResponseFactory factory) => _factory = factory;

    public static Response Render(string component) => _factory.Render(component);
    
    public static Response Render(string component, InertiaProps? props) => _factory.Render(component, props);
    
    public static Response Render(string component, Dictionary<string, object?>? props) => _factory.Render(component, InertiaProps.Create(props));

    public static Task<IHtmlContent> Head(InertiaPage model) => _factory.Head(model);

    public static Task<IHtmlContent> Html(InertiaPage model) => _factory.Html(model);

    public static void SetVersion(object? version) => _factory.SetVersion(version);

    public static string? GetVersion() => _factory.GetVersion();

    public static LocationResult Location(string url) => _factory.Location(url);
    public static InertiaBackResult Back() => _factory.Back();

    public static void Share(string key, object? value) => _factory.Share(key, value);

    public static void Share(InertiaProps data) => _factory.Share(data);
    
    public static void Flash(string key, string? value) => _factory.Flash(key, value);
    
    public static void EnableEncryptHistory(bool enable = true) => _factory.EnableEncryptHistory(enable);
    public static void ClearHistory() => _factory.ClearHistory();

    public static LazyProp<T> Lazy<T>(Func<T?> callback) => _factory.Lazy(callback);
    public static LazyProp<T> Lazy<T>(Func<Task<T?>> callback) => _factory.Lazy(callback);

    public static DeferredProp<T> Defer<T>(Func<T?> callback, string? group = null) => _factory.Defer(callback, group);
    public static DeferredProp<T> Defer<T>(Func<Task<T?>> callback, string? group = null) => _factory.Defer(callback, group);
    
    public static AlwaysProp<T> Always<T>(Func<T?> callback) => _factory.Always(callback);
    public static AlwaysProp<T> Always<T>(Func<Task<T?>> callback) => _factory.Always(callback);
    
    public static MergeProp<T> Merge<T>(Func<T?> callback) => _factory.Merge(callback);
    public static MergeProp<T> Merge<T>(Func<Task<T?>> callback) => _factory.Merge(callback);
}
