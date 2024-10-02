namespace InertiaNetCore.Utils;

internal interface IInvokableProp
{
    internal Task<object?> InvokeToObject();
}

public abstract class InvokableProp<T>: IInvokableProp
{
    private readonly Func<T?>? _callback;
    private readonly Func<Task<T?>>? _callbackAsync;

    protected InvokableProp(Func<T?> callback)
    {
        _callback = callback;
    }

    protected InvokableProp(Func<Task<T?>> callbackAsync)
    {
        _callbackAsync = callbackAsync;
    }
    
    public async Task<T?> Invoke()
    {
        if (_callback is not null)
            return _callback.Invoke();
        
        if (_callbackAsync is not null)
            return await _callbackAsync.Invoke();
        
        return default;
    }

    public async Task<object?> InvokeToObject()
    {
        return await Invoke();
    }
}