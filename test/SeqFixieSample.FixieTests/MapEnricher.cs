using Serilog.Core;
using Serilog.Events;

namespace SeqFixieSample.Tests;

public class MapEnricher : ILogEventEnricher
{
    readonly Dictionary<string, (object value, bool destructure)> _props = new();

    public void Set(string prop, object value, bool destructure = false)
    {
        ArgumentNullException.ThrowIfNull(prop);
        ArgumentNullException.ThrowIfNull(value);
        
        _props[prop] = (value, destructure);
    }
    
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach(var key in _props.Keys) {
            var prop = _props[key];
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(key, prop.value, prop.destructure));
        }
    }
}