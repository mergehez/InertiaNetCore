namespace InertiaNetCore.Utils;

/// <summary>
/// NEVER included on standard visits <br/>
/// OPTIONALLY included on partial reloads (you should call <c>router.reload({ only: ["propName"] })</c>) <br/>
/// ONLY evaluated when needed
/// </summary>
public class LazyProp(Func<object?> callback)
{
    public object? Invoke() => callback.Invoke();
}
