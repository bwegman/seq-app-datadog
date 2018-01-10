using System;
using System.Collections.Generic;
using DatadogSharp.DogStatsd;
using Moq;
using Seq.Apps;
using Seq.Apps.LogEvents;
using Xunit;
using Xunit.Abstractions;

namespace Seq.App.Datadog.Tests.DatadogReactorTests
{
    public class CollectorTests : ReactorFixture
    {
        private readonly List<Mock<IDatadogStats>> _mocks;
        private readonly List<Mock<IDisposable>> _disposables;

        private static readonly int LogLevelCount = Enum.GetValues(typeof(LogEventLevel)).Length;

        public CollectorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _mocks = new List<Mock<IDatadogStats>>();
            _disposables = new List<Mock<IDisposable>>();

            DatadogReactor.CreateCollector = (tags, level) =>
            {
                var mock = new Mock<IDatadogStats>();
                var disposable = mock.As<IDisposable>();

                _mocks.Add(mock);
                _disposables.Add(disposable);

                return mock.Object;
            };
        }

        [Fact]
        public void DoesNotCreateCollectorsOnCreate()
        {
            using (var reactor = new DatadogReactor())
            {
            }

            Assert.Empty(_mocks);
        }

        [Fact]
        public void CreatesCollectorsOnAttach()
        {
            using (var reactor = new DatadogReactor())
            {
                reactor.Attach(Mock.Of<IAppHost>());
            }

            Assert.Equal(LogLevelCount, _mocks.Count);
        }

        [Fact]
        public void DisposesCollectors()
        {
            using (var reactor = new DatadogReactor())
            {
                reactor.Attach(Mock.Of<IAppHost>());
            }

            Assert.Equal(Enum.GetValues(typeof(LogEventLevel)).Length, _disposables.Count);

            foreach(var disposable in _disposables)
                disposable.Verify(x => x.Dispose(), Times.Once);
        }
    }
}