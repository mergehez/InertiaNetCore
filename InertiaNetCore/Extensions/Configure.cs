using System.Net;
using InertiaNetCore.Models;
using InertiaNetCore.Ssr;
using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InertiaNetCore.Extensions;

public static class Configure
{
    public static IApplicationBuilder UseInertia(this IApplicationBuilder app)
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
        
        app.UseSession();

        return app;
    }

    public static IServiceCollection AddInertia(this IServiceCollection services,
        Action<InertiaOptions>? options = null)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddSingleton<SsrGateway>();
        services.AddSingleton<ResponseFactory>();


        services.Configure<MvcOptions>(mvcOptions => { mvcOptions.Filters.Add<InertiaActionFilter>(); });

        if (options != null)
        {
            services.Configure(options);
        }

        var opts = services.BuildServiceProvider().GetRequiredService<IOptions<InertiaOptions>>().Value;
        services.AddSession(opts.ConfigureSession);

        return services;
    }

    
    public static IApplicationBuilder AddInertiaSharedData(this IApplicationBuilder app, Func<HttpContext, Task<InertiaProps>> middleware)
    {
        app.Use(async (context, next) =>
        {
            var data = await middleware(context);
            
            Inertia.Share(data);
            
            await next();
        });
        
        return app;
    }
    
    public static IApplicationBuilder AddInertiaSharedData(this IApplicationBuilder app, Func<HttpContext, InertiaProps> middleware)
    {
        app.Use(async (context, next) =>
        {
            var data = middleware(context);
            
            Inertia.Share(data);
            
            await next();
        });
        
        return app;
    }
    
    public static IServiceCollection AddViteHelper(this IServiceCollection services, Action<ViteOptions>? options = null)
    {
        services.AddSingleton<IViteBuilder, ViteBuilder>();
        if (options != null) services.Configure(options);

        return services;
    }

    private static async Task OnVersionChange(HttpContext context, IApplicationBuilder app)
    {
        var tempData = app.ApplicationServices.GetRequiredService<ITempDataDictionaryFactory>()
            .GetTempData(context);

        if (tempData.Any()) tempData.Keep();

        context.Response.Headers.Append("X-Inertia-Location", context.RequestedUri());
        context.Response.StatusCode = (int)HttpStatusCode.Conflict;

        await context.Response.CompleteAsync();
    }
}
