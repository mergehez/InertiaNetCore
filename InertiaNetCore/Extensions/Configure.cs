using System.Net;
using InertiaNetCore.Models;
using InertiaNetCore.Ssr;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace InertiaNetCore.Extensions;

public static class Configure
{
    public static InertiaApplicationBuilder UseInertia(this IApplicationBuilder app)
    {
        var factory = app.ApplicationServices.GetRequiredService<ResponseFactory>();
        Inertia.UseFactory(factory);

        var viteBuilder = app.ApplicationServices.GetService<IViteBuilder>();
        if (viteBuilder != null) 
            Vite.UseBuilder(viteBuilder);

        app.Use(async (context, next) =>
        {
            if (context.IsInertiaRequest()
                && context.Request.Method == "GET"
                && context.Request.Headers["X-Inertia-Version"] != Inertia.GetVersion())
            {
                await OnVersionChange(context, app);
                return;
            }

            await next();
        });

        return new InertiaApplicationBuilder(app);
    }

    public static IServiceCollection AddInertia(this IServiceCollection services,
        Action<InertiaOptions>? options = null)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddSingleton<SsrGateway>();
        services.AddSingleton<ResponseFactory>();

        services.Configure<MvcOptions>(mvcOptions => { mvcOptions.Filters.Add<InertiaActionFilter>(); });

        if (options != null) services.Configure(options);

        return services;
    }

    public static IServiceCollection AddViteHelper(this IServiceCollection services, Action<ViteOptions>? options = null)
    {
        services.AddSingleton<IViteBuilder, ViteBuilder>();
        if (options != null) services.Configure(options);

        return services;
    }

    private static async Task OnVersionChange(HttpContext context, IApplicationBuilder app)
    {
        var tempData = app.ApplicationServices.GetRequiredService<TempDataDictionaryFactory>()
            .GetTempData(context);

        if (tempData.Any()) tempData.Keep();

        context.Response.Headers.Append("X-Inertia-Location", context.RequestedUri());
        context.Response.StatusCode = (int)HttpStatusCode.Conflict;

        await context.Response.CompleteAsync();
    }
}
