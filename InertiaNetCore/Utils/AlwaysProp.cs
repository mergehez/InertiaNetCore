namespace InertiaNetCore.Utils;

/// <summary>
/// ALWAYS included on standard visits <br/>
/// ALWAYS included on partial reloads <br/>
/// ALWAYS evaluated when needed
/// </summary>
public class AlwaysProp(Func<object?> callback)
{
    public object? Invoke() => callback.Invoke();
}
