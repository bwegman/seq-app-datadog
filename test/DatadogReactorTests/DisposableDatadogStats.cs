using System;
using DatadogSharp.DogStatsd;
using Moq;

namespace Seq.App.Datadog.Tests.DatadogReactorTests
{
    public class DisposableDatadogStats : Mock<IDatadogStats>, IDisposable
    {
        public void Dispose()
        {
        }
    }
}