using Microsoft.AspNetCore.Html;

namespace InertiaNetCore.Utils;

public interface IViteBuilder
{
    HtmlString ReactRefresh();
    HtmlString Input(string path);
}

internal interface IInvokableProp
{
    internal Task<object?> InvokeToObject();
}

internal interface IAlwaysProp;

internal interface IIgnoreFirstProp;

internal interface ILazyProp : IIgnoreFirstProp;

internal interface IDeferredProp : IIgnoreFirstProp
{
    string? Group { get; }
}