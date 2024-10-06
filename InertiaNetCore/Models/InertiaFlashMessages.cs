using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace InertiaNetCore.Models;

internal class InertiaFlashMessages(ISession? session)
{
    private InertiaProps Data { get; set; } = new();

    public InertiaProps GetData() => Data;

    public void Set(string key, object? value)
    {
        Data[key] = value;
        
        session?.SetString("flash", JsonSerializer.Serialize(Data));
    }
    
    public void Merge(Dictionary<string, string>? with)
    {
        if (with is null) 
            return;
        
        foreach (var (key, value) in with)
        {
            Data[key] = value;
        }
        
        session?.SetString("flash", JsonSerializer.Serialize(Data));
    }
    
    public void Clear(bool clearData = true)
    {
        if (clearData)
            Data.Clear();
        
        session?.Remove("flash");
    }
    
    public static InertiaFlashMessages FromSession(HttpContext httpContext)
    {
        try
        {
            var flash = new InertiaFlashMessages(httpContext.Session);
            var sessionData = httpContext.Session.GetString("flash");
            if (sessionData is not null)
            {
                flash.Data = JsonSerializer.Deserialize<InertiaProps>(sessionData) ?? new InertiaProps();
            }
        
            return flash;
        }
        catch (InvalidOperationException)
        {
            return new InertiaFlashMessages(null);
        }
        
    }
}
