using InertiaNetCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace InertiaNetCore.Utils;

public class InertiaApplicationBuilder(IApplicationBuilder app): IApplicationBuilder
{
    public IServiceProvider ApplicationServices { get; set; } = app.ApplicationServices;
    public IFeatureCollection ServerFeatures { get; } = app.ServerFeatures;
    public IDictionary<string, object?> Properties { get; } = app.Properties;
    
    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => app.Use(middleware);

    public IApplicationBuilder New() => app.New();

    public RequestDelegate Build() => app.Build();

    public InertiaApplicationBuilder AddSharedData(Func<HttpContext, Task<InertiaProps>> middleware)
    {
        app.Use(async (context, next) =>
        {
            var data = await middleware(context);
            
            Inertia.Share(data);
            
            await next();
        });
        
        return this;
    }
    public InertiaApplicationBuilder AddSharedData(Func<HttpContext, InertiaProps> middleware)
    {
        app.Use(async (context, next) =>
        {
            var data = middleware(context);
            
            Inertia.Share(data);
            
            await next();
        });
        
        return this;
    }
}