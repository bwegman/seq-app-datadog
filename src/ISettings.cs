namespace Seq.App.Datadog
{
    public interface ISettings
    {
        string DatadogApiKey { get; }
        string ApplicationProperty { get; }
        string Tags { get; }
    }
}