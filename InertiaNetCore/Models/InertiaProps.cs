using InertiaNetCore.Utils;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InertiaNetCore.Models;

public class InertiaProps : Dictionary<string, object?>
{
    internal InertiaProps ToProcessedProps(List<string>? partials)
    {
        var props = new InertiaProps();
        
        if(partials is not null && partials.Count == 0)
            partials = null;

        foreach (var (key, value) in this)
        {
            if(partials is null && value is LazyProp)
                continue;
            
            if(partials is not null && value is not AlwaysProp && !partials.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                continue;
            
            props.Add(key, value switch
            {
                Func<object?> f => f.Invoke(),
                Delegate d => d.DynamicInvoke(),
                LazyProp l => l.Invoke(),
                AlwaysProp l => l.Invoke(),
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
    internal InertiaProps AddTimeStamp()
    {
        this["timestamp"] = DateTime.UtcNow;
        return this;
    }
}