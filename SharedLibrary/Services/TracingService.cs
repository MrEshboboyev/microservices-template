using System.Diagnostics;

namespace SharedLibrary.Services;

public class TracingService : ITracingService
{
    private static readonly ActivitySource ActivitySource = new("SharedLibrary", "1.0.0");

    public Activity StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return ActivitySource.StartActivity(name, kind);
    }

    public Activity StartActivity(string name, IEnumerable<KeyValuePair<string, object>> tags, ActivityKind kind = ActivityKind.Internal)
    {
        var activity = ActivitySource.StartActivity(name, kind);
        
        if (activity != null && tags != null)
        {
            foreach (var tag in tags)
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        }
        
        return activity;
    }

    public void AddTag(string key, string value)
    {
        CurrentActivity?.SetTag(key, value);
    }

    public void AddEvent(string name)
    {
        CurrentActivity?.AddEvent(new ActivityEvent(name));
    }

    public void AddEvent(string name, IEnumerable<KeyValuePair<string, object>> attributes)
    {
        var tags = new ActivityTagsCollection();
        
        if (attributes != null)
        {
            foreach (var attribute in attributes)
            {
                tags[attribute.Key] = attribute.Value;
            }
        }
        
        CurrentActivity?.AddEvent(new ActivityEvent(name, tags: tags));
    }

    public void SetStatus(string description, ActivityStatusCode code = ActivityStatusCode.Ok)
    {
        CurrentActivity?.SetStatus(code, description);
    }

    public Activity CurrentActivity => Activity.Current;
}
