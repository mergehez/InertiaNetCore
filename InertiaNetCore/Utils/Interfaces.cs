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

internal interface IDeferredProp : IIgnoreFirstProp, IMergeableProp
{
    string? Group { get; }
}

public interface IMergeableProp
{
    bool Merge { get; set; }
}
