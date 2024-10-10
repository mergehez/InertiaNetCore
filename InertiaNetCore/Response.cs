using System.Text.Json;
using InertiaNetCore.Extensions;
using InertiaNetCore.Models;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace InertiaNetCore;

public class Response(string component, InertiaProps props, string? version, InertiaOptions options)
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
            DeferredProps = GetDeferredProps(context),
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

            await new ViewResult { ViewName = options.RootView, ViewData = viewData }.ExecuteResultAsync(context);
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
    
    private Dictionary<string, List<string>> GetDeferredProps(ActionContext context)
    {
        if (context.IsInertiaPartialComponent(component))
            return [];

        var tmp = new Dictionary<string, string>();
        foreach (var (key, value) in props)
        {
            if (value is IDeferredProp deferredProp)
                tmp[key] = deferredProp.Group ?? $"{key}_{Random.Shared.Next()}";
        }
        
        // apply json serialization options to dictionary keys before grouping them
        var jsonOptions = options.JsonSerializerOptions as JsonSerializerOptions;
        tmp = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(tmp, jsonOptions), jsonOptions);
        
        return tmp!
            .GroupBy(prop => prop.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Key).ToList()
            );
    }
    
    private static Dictionary<string, string> GetErrors(ActionContext context)
    {
        var sessionErrors = context.HttpContext.Session.GetString("errors");
        if (sessionErrors is not null)
        {
            var errors = JsonSerializer.Deserialize<Dictionary<string, string>>(sessionErrors);
            context.HttpContext.Session.Remove("errors");
            
            if(errors is not null)
                return errors;
        }
        
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
