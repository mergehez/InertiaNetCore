namespace InertiaNetCore.Utils;


/// <summary>
/// ALWAYS included on standard visits <br/>
/// ALWAYS included on partial reloads <br/>
/// ALWAYS evaluated when needed
/// </summary>
public class AlwaysProp<T> : InvokableProp<T>, IAlwaysProp
{
    public AlwaysProp(Func<T?> callback) : base(callback)
    {
    }

    public AlwaysProp(Func<Task<T?>> callbackAsync) : base(callbackAsync)
    {
    }
}