using InertiaNetCore.Extensions;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InertiaNetCore;

public class Response(string component, object props, string rootView, string? version, Func<InertiaPage?, Task<IActionResult>> jsonResultResolver)
    : IActionResult
{
    private InertiaPage? _page;
    private IDictionary<string, object>? _viewData;
    
    public async Task ExecuteResultAsync(ActionContext context)
    {
        ProcessResponse(context);

        if (!context.IsInertiaRequest())
        {
            await GetView(context).ExecuteResultAsync(context);
        }
        else
        {
            context.HttpContext.Response.Headers.Append("X-Inertia", "true");
            context.HttpContext.Response.Headers.Append("Vary", "Accept");
            context.HttpContext.Response.StatusCode = 200;
            await (await jsonResultResolver.Invoke(_page)).ExecuteResultAsync(context);
        }
    }

    protected internal void ProcessResponse(ActionContext context)
    {
        var page = new InertiaPage
        {
            Component = component,
            Version = version,
            Url = context.RequestedUri()
        };

        var partial = context.GetPartialData();
        if (partial.Count != 0 && context.IsInertiaPartialComponent(component))
        {
            page.Props = props.GetType()
                .GetProperties()
                .Select(c => c.Name)
                .Intersect(partial, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(o => o.ToCamelCase(), o => props.GetType().GetProperty(o)?.GetValue(props));
        }
        else
        {
            page.Props = props.GetType().GetProperties()
                .Where(o => o.PropertyType != typeof(LazyProp))
                .ToDictionary(o => o.Name.ToCamelCase(), o => o.GetValue(props));
        }

        page.Props = PrepareProps(page.Props);

        var shared = context.HttpContext.Features.Get<InertiaSharedData>();
        if (shared is not null)
            page.Props = shared.GetMerged(page.Props);

        page.Props["errors"] = GetErrors(context);

        _page = page;
    }

    private static Dictionary<string, object?> PrepareProps(Dictionary<string, object?> props)
    {
        return props.ToDictionary(pair => pair.Key, pair => pair.Value switch
        {
            Func<object?> f => f.Invoke(),
            LazyProp l => l.Invoke(),
            _ => pair.Value
        });
    }

    protected internal InertiaPage? GetPage()
    {
        return _page;
    }

    protected internal ViewResult GetView(ActionContext context)
    {
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
        {
            Model = _page
        };

        if (_viewData == null) return new ViewResult { ViewName = rootView, ViewData = viewData };

        foreach (var (key, value) in _viewData)
            viewData[key] = value;

        return new ViewResult { ViewName = rootView, ViewData = viewData };
    }
    
    // protected internal IActionResult GetResult() => _context!.IsInertiaRequest() 
    //     ? GetPage() 
    //     : GetView();

    private IDictionary<string, string> GetErrors(ActionContext context)
    {
        if (!context.ModelState.IsValid)
            return context.ModelState.ToDictionary(o => o.Key.ToCamelCase(),
                o => o.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? "");

        return new Dictionary<string, string>(0);
    }

    public Response WithViewData(IDictionary<string, object> viewData)
    {
        _viewData = viewData;
        return this;
    }
}
