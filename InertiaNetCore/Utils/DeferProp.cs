namespace InertiaNetCore.Utils;

/// <summary>
/// Deferred props allow you to defer the loading of certain page data until after the initial page render.
/// This can be useful for improving the perceived performance of your app by allowing the initial page render to happen as quickly as possible.
/// </summary>
public class DeferredProp<T> : InvokableProp<T>, IDeferredProp
{
    public string? Group { get; }

    public DeferredProp(Func<T?> callback, string? group) : base(callback)
    {
        Group = group;
    }

    public DeferredProp(Func<Task<T?>> callbackAsync, string? group) : base(callbackAsync)
    {
        Group = group;
    }
}