using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using InertiaNetCore.Models;
using InertiaNetCore.Ssr;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InertiaNetCore;

internal interface IResponseFactory
{
    public Response Render(string component, object? props = null);
    public Task<IHtmlContent> Head(dynamic model);
    public Task<IHtmlContent> Html(dynamic model);
    public void Version(object? version);
    public string? GetVersion();
    public LocationResult Location(string url);
    public void Share(string key, object? value);
    public void Share(IDictionary<string, object?> data);
    public LazyProp Lazy(Func<object?> callback);
}

internal class ResponseFactory(IHttpContextAccessor contextAccessor, IGateway gateway, IOptions<InertiaOptions> options) : IResponseFactory
{
    private object? _version;

    public Response Render(string component, object? props = null)
    {
        props ??= new { };

        return new Response(component, props, options.Value.RootView, GetVersion(), options.Value.JsonResultResolver);
    }

    public async Task<IHtmlContent> Head(dynamic model)
    {
        if (!options.Value.SsrEnabled) return new HtmlString("");

        var context = contextAccessor.HttpContext!;

        var response = context.Features.Get<SsrResponse>();
        response ??= await gateway.Dispatch(model, options.Value.SsrUrl);

        if (response == null) return new HtmlString("");

        context.Features.Set(response);
        return response.GetHead();
    }

    public async Task<IHtmlContent> Html(dynamic model)
    {
        if (options.Value.SsrEnabled)
        {
            var context = contextAccessor.HttpContext!;

            var response = context.Features.Get<SsrResponse>();
            response ??= await gateway.Dispatch(model, options.Value.SsrUrl);

            if (response != null)
            {
                context.Features.Set(response);
                return response.GetBody();
            }
        }

        var data = JsonSerializer.Serialize(model,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

        var encoded = WebUtility.HtmlEncode(data);

        return new HtmlString($"<div id=\"app\" data-page=\"{encoded}\"></div>");
    }

    public void Version(object? version) => _version = version;

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

        var sharedData = context.Features.Get<InertiaSharedData>();
        sharedData ??= new InertiaSharedData();
        sharedData.Set(key, value);

        context.Features.Set(sharedData);
    }

    public void Share(IDictionary<string, object?> data)
    {
        var context = contextAccessor.HttpContext!;

        var sharedData = context.Features.Get<InertiaSharedData>();
        sharedData ??= new InertiaSharedData();
        sharedData.Merge(data);

        context.Features.Set(sharedData);
    }

    public LazyProp Lazy(Func<object?> callback) => new(callback);
    public AlwaysProp Always(Func<object?> callback) => new(callback);
}
