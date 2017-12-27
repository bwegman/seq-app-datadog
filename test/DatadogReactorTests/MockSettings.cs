namespace Seq.App.Datadog.Tests.CanaryReactorTests
{
    public class MockSettings : ISettings
    {
        public string DatadogApiKey { get; } = TestConstants.ApiKey;
        
        public string ApplicationProperty { get; } = TestConstants.ApplicationName;

        public string Tags { get; } = TestConstants.Tags;
    }
}