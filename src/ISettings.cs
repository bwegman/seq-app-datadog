namespace Seq.App.Datadog
{
    public interface ISettings
    {
        string MetricName { get; }
        string DefaultTags { get; }
    }
}