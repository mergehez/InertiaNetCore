using System.Net;
using InertiaNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InertiaNetCore.Utils;

/// <summary>
/// Returns back to the page, that made the request. The page typically made a POST, PUT, PATCH or DELETE request. 
/// see doc: https://inertiajs.com/redirects#303-response-code
/// </summary>
public class InertiaBackResult : IActionResult
{
    private readonly HttpContext _httpContext;

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly string _url;

    /// <inheritdoc cref="InertiaBackResult"/>
    public InertiaBackResult(HttpContext httpContext)
    {
        _httpContext = httpContext;
        var referer = httpContext.Request.Headers.Referer.ToString();
        _url = string.IsNullOrWhiteSpace(referer) ? "/" : referer;
    }

    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.Clear();
        context.HttpContext.Response.Headers.Append("X-Inertia-Location", _url);
        context.HttpContext.Response.Redirect(_url);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.SeeOther;
        
        return Task.CompletedTask;
    }
    
    
    public InertiaBackResult WithFlash(string key, string? value)
    {
        var flush = _httpContext.Features.Get<InertiaFlashMessages>() 
                    ?? InertiaFlashMessages.FromSession(_httpContext);
        flush.Set(key, value);
        _httpContext.Features.Set(flush);
        
        return this;
    }
    
    public InertiaBackResult WithFlash(Dictionary<string, string> flash)
    {
        var flush = _httpContext.Features.Get<InertiaFlashMessages>() 
                    ?? InertiaFlashMessages.FromSession(_httpContext);
        flush.Merge(flash);
        _httpContext.Features.Set(flush);
        return this;
    }
}
