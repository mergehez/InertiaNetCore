using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;

namespace InertiaNetCore.Models;

public record InertiaJsonOptions
{
    private InertiaJsonOptions()
    {
    }

    internal object Options { get; private init; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    private Func<object, object, string> SerializeFn { get; init; } = (model, options)
        => JsonSerializer.Serialize(model, options as JsonSerializerOptions);

    internal string Serialize(object model) => SerializeFn(model, Options);

    public static InertiaJsonOptions Default { get; } = new();

    public static InertiaJsonOptions Create<T>(T options, Func<object, T, string>? serialize = null) where T : class
    {
        serialize ??= (model, jo) => JsonSerializer.Serialize(model, jo as JsonSerializerOptions);
        return new InertiaJsonOptions
        {
            Options = options,
            SerializeFn = (model, jo) => serialize(model, (jo as T)!)
        };
    }
}

public class InertiaOptions
{
    public string RootView { get; set; } = "~/Views/App.cshtml";

    public bool SsrEnabled { get; set; } = false;
    public string SsrUrl { get; set; } = "http://127.0.0.1:13714/render";

    public Action<SessionOptions> ConfigureSession { get; set; } = _ => { };

    public bool EncryptHistory { get; set; }

    public InertiaJsonOptions Json { get; set; } = InertiaJsonOptions.Default;
}