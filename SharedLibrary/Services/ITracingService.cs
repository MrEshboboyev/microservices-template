using System.Diagnostics;

namespace SharedLibrary.Services;

public interface ITracingService
{
    /// <summary>
    /// Start a new activity (trace span)
    /// </summary>
    /// <param name="name">Name of the activity</param>
    /// <param name="kind">Activity kind</param>
    /// <returns>Activity instance</returns>
    Activity StartActivity(string name, ActivityKind kind = ActivityKind.Internal);

    /// <summary>
    /// Start a new activity with tags
    /// </summary>
    /// <param name="name">Name of the activity</param>
    /// <param name="tags">Tags to add to the activity</param>
    /// <param name="kind">Activity kind</param>
    /// <returns>Activity instance</returns>
    Activity StartActivity(string name, IEnumerable<KeyValuePair<string, object>> tags, ActivityKind kind = ActivityKind.Internal);

    /// <summary>
    /// Add a tag to the current activity
    /// </summary>
    /// <param name="key">Tag key</param>
    /// <param name="value">Tag value</param>
    void AddTag(string key, string value);

    /// <summary>
    /// Add an event to the current activity
    /// </summary>
    /// <param name="name">Event name</param>
    void AddEvent(string name);

    /// <summary>
    /// Add an event with attributes to the current activity
    /// </summary>
    /// <param name="name">Event name</param>
    /// <param name="attributes">Event attributes</param>
    void AddEvent(string name, IEnumerable<KeyValuePair<string, object>> attributes);

    /// <summary>
    /// Set status of the current activity
    /// </summary>
    /// <param name="description">Status description</param>
    /// <param name="code">Status code</param>
    void SetStatus(string description, ActivityStatusCode code = ActivityStatusCode.Ok);

    /// <summary>
    /// Get the current activity
    /// </summary>
    Activity CurrentActivity { get; }
}
