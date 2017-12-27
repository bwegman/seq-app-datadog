using System;
using Seq.Apps;
using Seq.Apps.LogEvents;

namespace Seq.App.Datadog
{
    [SeqApp(
        "Datadog reporting",
         Description = "Reports counts of events tagged by loglevel to Datadog instance."
    )]
    public class DatadogReactor : Reactor, ISubscribeTo<LogEventData>, ISettings, IDisposable
    {
        [SeqAppSetting(
            DisplayName = "Datadog API key",
            HelpText = "Datadog API key")]
        public string DatadogApiKey { get; set; }

        [SeqAppSetting(
             DisplayName = "Metric name",
             HelpText = "The name of the metric to use in Datadog")]
        public string ApplicationProperty { get; set; }

        [SeqAppSetting(
             DisplayName = "Tags to apply to metrics",
             HelpText = "List of tags to always apply to the metrics. Each tag should be on a separate line.",
             InputType = SettingInputType.LongText)]
        public string Tags { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        public void On(Event<LogEventData> evt)
        {

        }

        public void Dispose()
        {
        }
    }
}