using System.Text.Json;
using System.Text.Json.Serialization;

namespace InertiaNetCore.Models;

public class InertiaOptions
{
    public string RootView { get; set; } = "~/Views/App.cshtml";

    public bool SsrEnabled { get; set; } = false;
    public string SsrUrl { get; set; } = "http://127.0.0.1:13714/render";
    
    public bool SessionEnabled { get; set; } = false;

    private static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    public object JsonSerializerOptions { get; set; } = DefaultJsonSerializerOptions;
    
    public Func<object, string> JsonSerializeFn { get; set; } = model => JsonSerializer.Serialize(model, DefaultJsonSerializerOptions);
}
