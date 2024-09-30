using System.Net;
using InertiaNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace InertiaNetCore.Utils;

internal class InertiaActionFilter(IUrlHelperFactory urlHelperFactory) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        //
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var methods = (ReadOnlySpan<string>)["PUT", "PATCH", "DELETE"];
        if (!context.HttpContext.IsInertiaRequest() || !methods.Contains(context.HttpContext.Request.Method)) 
            return;

        var destinationUrl = context.Result switch
        {
            RedirectResult result => result.Url,
            RedirectToActionResult result => GetUrl(result, context),
            RedirectToPageResult result => GetUrl(result, context),
            RedirectToRouteResult result => GetUrl(result, context),
            _ => null
        };

        if (destinationUrl == null) return;
        context.HttpContext.Response.Headers.Append("Location", destinationUrl);
        context.Result = new StatusCodeResult((int)HttpStatusCode.RedirectMethod);
    }

    private string? GetUrl(RedirectToActionResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? urlHelperFactory.GetUrlHelper(context);

        return urlHelper.Action(
            result.ActionName,
            result.ControllerName,
            result.RouteValues,
            null,
            null,
            result.Fragment);
    }

    private string? GetUrl(RedirectToPageResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? urlHelperFactory.GetUrlHelper(context);

        return urlHelper.Page(
            result.PageName,
            result.PageHandler,
            result.RouteValues,
            result.Protocol,
            result.Host,
            result.Fragment);
    }

    private string? GetUrl(RedirectToRouteResult result, ActionContext context)
    {
        var urlHelper = result.UrlHelper ?? urlHelperFactory.GetUrlHelper(context);

        return urlHelper.RouteUrl(
            result.RouteName,
            result.RouteValues,
            null,
            null,
            result.Fragment);
    }
}
