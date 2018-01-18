using System;
using System.Linq;
using Seq.Apps;
using Seq.Apps.LogEvents;
using StatsdClient;

namespace Seq.App.Datadog
{
    [SeqApp(
        "Datadog reporting",
         Description = "Reports counts of events tagged by loglevel to Datadog instance."
    )]
    public class DatadogReactor : Reactor, ISubscribeTo<LogEventData>, ISettings, IDisposable
    {
        [SeqAppSetting(
             DisplayName = "Metric name",
             HelpText = "The name of the metric to use in Datadog")]
        public string MetricName { get; set; }

        [SeqAppSetting(
            DisplayName = "Tags to apply to metrics",
            HelpText = "List of tags to always apply to the metrics. Each tag should be on a separate line.",
            InputType = SettingInputType.LongText)]
        public string DefaultTags { get; set; } = string.Empty;

        private string[] _tagArray;

        protected override void OnAttached()
        {
            base.OnAttached();

            DogStatsd.Configure(new StatsdConfig
            {
                StatsdServerName = "localhost",
            });

            _tagArray = DefaultTags.Split('\n');
        }

        public void On(Event<LogEventData> evt)
        {
            var tags = GetTags(evt);

            DogStatsd.Increment(MetricName, tags: tags);
        }

        private string[] GetTags(Event<LogEventData> evt)
        {
            var levelTag = $"level:{evt.Data.Level.ToString().ToLowerInvariant()}";

            return _tagArray.Concat(new[] {levelTag}).ToArray();
        }

        public void Dispose()
        {
        }
    }
}