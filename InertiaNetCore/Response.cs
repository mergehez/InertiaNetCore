using InertiaNetCore.Extensions;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InertiaNetCore;

public class Response(string component, InertiaProps props, string rootView, string? version, object jsonSerializerOptions)
    : IActionResult
{
    private IDictionary<string, object>? _viewData;
    
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var page = new InertiaPage
        {
            Component = component,
            Version = version,
            Url = context.HttpContext.RequestedUri(),
            Props = GetFinalProps(context),
        };
        
        if (!context.HttpContext.IsInertiaRequest())
        {
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
            {
                Model = page
            };

            if (_viewData is not null)
            {
                foreach (var (key, value) in _viewData)
                    viewData[key] = value;
            }

            await new ViewResult { ViewName = rootView, ViewData = viewData }.ExecuteResultAsync(context);
        }
        else
        {
            context.HttpContext.Response.Headers.Append("X-Inertia", "true");
            context.HttpContext.Response.Headers.Append("Vary", "Accept");
            context.HttpContext.Response.StatusCode = 200;
            
            var jsonResult = new JsonResult(page, jsonSerializerOptions);
            await jsonResult.ExecuteResultAsync(context);
        }
    }
    
    private InertiaProps GetFinalProps(ActionContext context)
    {
        var partials = context.IsInertiaPartialComponent(component) ? context.GetPartialData() : null;
        var shared = context.HttpContext.Features.Get<InertiaSharedProps>();
        var errors = GetErrors(context);
        
        return props
            .ToProcessedProps(partials)
            .Merge(shared?.GetData())
            .AddTimeStamp()
            .AddErrors(errors);
    }
    
    private static Dictionary<string, string> GetErrors(ActionContext context)
    {
        if (context.ModelState.IsValid) 
            return [];
        
        return context.ModelState.ToDictionary(
            kv => kv.Key,
            kv => kv.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? ""
            );

    }

    public Response WithViewData(IDictionary<string, object> viewData)
    {
        _viewData = viewData;
        return this;
    }
}
