namespace InertiaNetCore.Utils;

/// <summary>
/// NEVER included on standard visits <br/>
/// OPTIONALLY included on partial reloads (you should call <c>router.reload({ only: ["propName"] })</c>) <br/>
/// ONLY evaluated when needed
/// </summary>
public class LazyProp<T> : InvokableProp<T>, ILazyProp
{
    public LazyProp(Func<T?> callback) : base(callback)
    {
    }

    public LazyProp(Func<Task<T?>> callbackAsync) : base(callbackAsync)
    {
    }
}