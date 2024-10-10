using InertiaNetCore.Utils;

namespace InertiaNetCore.Models;

public class InertiaProps : Dictionary<string, object?>
{
    internal async Task<InertiaProps> ToProcessedProps(List<string>? partials)
    {
        var props = new InertiaProps();
        
        if(partials is not null && partials.Count == 0)
            partials = null;

        foreach (var (key, value) in this)
        {
            if(partials is null && value is IIgnoreFirstProp)
                continue;
            
            if(partials is not null && value is not IAlwaysProp && !partials.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                continue;
            
            props.Add(key, value switch
            {
                Func<Task<object?>> f => await f.Invoke(),
                Func<object?> f => f.Invoke(),
                Delegate d => d.DynamicInvoke(),
                IInvokableProp l => await l.InvokeToObject(),
                _ => value
            });
        }

        return props;
    }
    
    internal static InertiaProps Create(Dictionary<string, object?>? initial)
    {
        return new InertiaProps().Merge(initial);
    }

    internal InertiaProps Merge(Dictionary<string, object?>? with)
    {
        if (with is null) 
            return this;
        
        foreach (var (key, value) in with)
        {
            this[key] = value;
        }
        
        return this;
    }
    
    internal InertiaProps AddErrors(IDictionary<string,string> errors)
    {
        this["errors"] = errors;
        return this;
    }
    internal InertiaProps AddFlash(InertiaProps? flash)
    {
        this["flash"] = flash;
        return this;
    }
    internal InertiaProps AddTimeStamp()
    {
        this["timestamp"] = DateTime.UtcNow;
        return this;
    }
}