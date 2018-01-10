using System;
using DatadogSharp.DogStatsd;
using Moq;
using Seq.Apps;
using Seq.Apps.LogEvents;
using Xunit;

namespace Seq.App.Datadog.Tests.DatadogReactorTests
{
    public class EventLoggingTests : IDisposable
    {
        private Mock<IDatadogStats> _dataDogMockForDebug;
        private readonly DatadogReactor _reactor;
        private readonly Func<string[], LogEventLevel, IDatadogStats> _mockCollectorFactory;

        public EventLoggingTests()
        {
            _mockCollectorFactory = (tags, level) =>
            {
                var mock = new Mock<IDatadogStats>();

                if (level == LogEventLevel.Debug)
                    _dataDogMockForDebug = mock;

                return mock.Object;
            };

            _reactor = new DatadogReactor(_mockCollectorFactory)
            {
                MetricName = TestConstants.MetricName
            };

            _reactor.Attach(Mock.Of<IAppHost>());
        }

        [Fact]
        public void LogsEventsToGivenMetric()
        {
            _reactor.On(ReactorFixture.EventForApplication(LogEventLevel.Debug));

            _dataDogMockForDebug.Verify(x => x.Increment(TestConstants.MetricName, 1, 1, It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public void LogsEventsToDatadogWithAssociatedLevel()
        {
            _reactor.On(ReactorFixture.EventForApplication(LogEventLevel.Debug));

            _dataDogMockForDebug.Verify(x => x.Increment(It.IsAny<string>(), 1, 1, It.IsAny<string[]>()), Times.Once());
        }

        [Fact]
        public void DoesNotLogExtraTags()
        {
            var tags = Array.Empty<string>();

            _dataDogMockForDebug.Setup(x => x.Increment(It.IsAny<string>(), 1, 1, It.IsAny<string[]>()))
                .Callback((string metric, long value, double sampleRate, string[] actualTags) =>
                {
                    tags = actualTags;
                });

            _reactor.On(ReactorFixture.EventForApplication(LogEventLevel.Debug));

            Assert.Null(tags);
        }

        public void Dispose()
        {
            _reactor?.Dispose();
        }
    }
}