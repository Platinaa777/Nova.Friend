using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Nova.Friend.Application.Monitoring;

public class ApplicationMetrics
{
    public const string ApplicationName = "Nova.Friend.App";
    private readonly Meter _meter = new(ApplicationName);
    private readonly ConcurrentDictionary<string, Counter<int>> _counters = new();
    public static readonly ActivitySource ActivitySource = new(ApplicationName);

    public void IncrementCount(string name, int value,
        IDictionary<string, object?>? additionalTags = null)
    {
        var counter = _counters.GetOrAdd(name, _meter.CreateCounter<int>(name));
        counter.Add(value, additionalTags?.ToArray() 
                           ?? ReadOnlySpan<KeyValuePair<string, object?>>.Empty);
    }

    public static IDictionary<string, object?> ResultTags(bool success) => 
        new Dictionary<string, object?>
    {
        ["success"] = success
    };
}