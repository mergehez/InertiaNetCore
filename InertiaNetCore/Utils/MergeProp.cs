namespace InertiaNetCore.Utils;

/// <summary>
/// By default, Inertia overwrites props with the same name when reloading a page.
/// However, there are instances, such as pagination or infinite scrolling, where that is not the desired behavior.
/// In these cases, you can merge props instead of overwriting them.
/// </summary>
public class MergeProp<T> : InvokableProp<T>, IMergeableProp
{
    public bool Merge { get; set; } = true;

    public MergeProp(Func<T?> callback) : base(callback)
    {
    }

    public MergeProp(Func<Task<T?>> callbackAsync) : base(callbackAsync)
    {
    }
}