namespace Seq.App.Datadog.Tests.CanaryReactorTests
{
    public class MockSettings : ISettings
    {
        public string MetricName { get; } = TestConstants.MetricName;

        public string DefaultTags { get; } = TestConstants.DefaultTags;
    }
}