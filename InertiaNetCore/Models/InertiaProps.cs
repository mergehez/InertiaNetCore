using InertiaNetCore.Utils;

namespace InertiaNetCore.Models;

public class InertiaProps : Dictionary<string, object?>
{
    internal async Task<InertiaProps> ToProcessedProps(bool isPartial, List<string> partials, List<string> excepts)
    {
        var props = new InertiaProps();
        
        foreach (var (key, value) in this)
        {
            if(isPartial && excepts.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                continue;
            
            if(!isPartial && value is IIgnoreFirstProp)
                continue;
            
            if(isPartial && value is not IAlwaysProp && !partials.Contains(key, StringComparer.InvariantCultureIgnoreCase))
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