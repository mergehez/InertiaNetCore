using InertiaNetCore.Extensions;
using InertiaNetCore.Models;
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
            Props = await GetFinalProps(context),
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
    
    private async Task<InertiaProps> GetFinalProps(ActionContext context)
    {
        var partials = context.IsInertiaPartialComponent(component) ? context.GetPartialData() : null;
        var shared = context.HttpContext.Features.Get<InertiaSharedProps>();
        var flash = context.HttpContext.Features.Get<InertiaFlashMessages>() 
                    ?? InertiaFlashMessages.FromSession(context.HttpContext);
        var errors = GetErrors(context);
        
        var finalProps = await props.ToProcessedProps(partials);
        
        finalProps = finalProps
            .Merge(shared?.GetData())
            .AddTimeStamp()
            .AddFlash(flash.GetData())
            .AddErrors(errors);
        
        flash.Clear(false);
        
        return finalProps;
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
