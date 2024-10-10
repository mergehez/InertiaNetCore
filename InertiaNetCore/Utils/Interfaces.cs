using Microsoft.AspNetCore.Html;

namespace InertiaNetCore.Utils;

internal interface IInvokableProp
{
    internal Task<object?> InvokeToObject();
}

internal interface IAlwaysProp;

internal interface ILazyProp;


public interface IViteBuilder
{
    HtmlString ReactRefresh();
    HtmlString Input(string path);
}