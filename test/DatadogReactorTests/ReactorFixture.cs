using System;
using System.IO;
using Moq;
using Seq.Apps;
using Seq.Apps.LogEvents;
using Serilog;
using Xunit.Abstractions;

namespace Seq.App.Datadog.Tests.DatadogReactorTests
{
    public abstract class ReactorFixture : IDisposable
    {
        private readonly ITestOutputHelper outputHelper;
        private readonly StringWriter log = new StringWriter();

        protected readonly StringWriter Messages = new StringWriter();
        protected readonly IAppHost AppHost;

        protected ReactorFixture(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;

            var log = new LoggerConfiguration()
                .WriteTo.TextWriter(this.log)
                .WriteTo.TextWriter(Messages, outputTemplate: "{Message}{NewLine}")
                .CreateLogger();

            var appHostMock = new Mock<IAppHost>();
            appHostMock.Setup(x => x.Logger).Returns(() => log);

            AppHost = appHostMock.Object;
        }

        public void Dispose()
        {
            outputHelper.WriteLine(log.ToString());
        }

        public static Event<LogEventData> EventForApplication(LogEventLevel level)
        {
            return new Event<LogEventData>("1", 0, DateTime.Now, new LogEventData
            {
                Level = level
            });
        }
    }
}