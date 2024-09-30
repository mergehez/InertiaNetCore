using System.Diagnostics.CodeAnalysis;
using System.Net;
using InertiaNetCore.Models;
using InertiaNetCore.Ssr;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InertiaNetCore;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
internal class ResponseFactory(IHttpContextAccessor contextAccessor, SsrGateway ssrGateway, IOptions<InertiaOptions> options)
{
    private object? _version;

    public Response Render(string component, InertiaProps? props = default)
    {
        props ??= [];

        return new Response(component, props, options.Value.RootView, GetVersion(), options.Value.JsonSerializerOptions);
    }

    public async Task<IHtmlContent> Head(dynamic model)
    {
        if (!options.Value.SsrEnabled) return new HtmlString("");

        var context = contextAccessor.HttpContext!;

        var response = context.Features.Get<SsrResponse?>();
        response ??= await ssrGateway.Dispatch(model, options.Value.SsrUrl);

        if (response.Value == default) 
            return new HtmlString("");

        context.Features.Set(response);
        return response.Value.GetHead();
    }

    public async Task<IHtmlContent> Html(dynamic model)
    {
        if (options.Value.SsrEnabled)
        {
            var context = contextAccessor.HttpContext!;

            var response = context.Features.Get<SsrResponse?>();
            response ??= await ssrGateway.Dispatch(model, options.Value.SsrUrl);

            if (response.Value != default)
            {
                context.Features.Set(response);
                return response.Value.GetBody();
            }
        }

        var data = options.Value.JsonSerializeFn(model);
        var encoded = WebUtility.HtmlEncode(data);

        return new HtmlString($"<div id=\"app\" data-page=\"{encoded}\"></div>");
    }

    public void SetVersion(object? version) => _version = version;

    public string? GetVersion() => _version switch
    {
        Func<string> func => func.Invoke(),
        string s => s,
        _ => null,
    };

    public LocationResult Location(string url) => new(url);

    public void Share(string key, object? value)
    {
        var context = contextAccessor.HttpContext!;

        var sharedData = context.Features.Get<InertiaSharedProps>();
        sharedData ??= new InertiaSharedProps();
        sharedData.Set(key, value);

        context.Features.Set(sharedData);
    }

    public void Share(InertiaProps data)
    {
        var context = contextAccessor.HttpContext!;

        var sharedData = context.Features.Get<InertiaSharedProps>();
        sharedData ??= new InertiaSharedProps();
        sharedData.Merge(data);

        context.Features.Set(sharedData);
    }

    public LazyProp Lazy(Func<object?> callback) => new(callback);
    public AlwaysProp Always(Func<object?> callback) => new(callback);
}
