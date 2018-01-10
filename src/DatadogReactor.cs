using System;
using System.Collections.Generic;
using System.Linq;
using DatadogSharp.DogStatsd;
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
             DisplayName = "Metric name",
             HelpText = "The name of the metric to use in Datadog")]
        public string MetricName { get; set; }

        [SeqAppSetting(
            DisplayName = "Tags to apply to metrics",
            HelpText = "List of tags to always apply to the metrics. Each tag should be on a separate line.",
            InputType = SettingInputType.LongText)]
        public string DefaultTags { get; set; } = string.Empty;

        private readonly Dictionary<LogEventLevel, IDatadogStats> _collectors = new Dictionary<LogEventLevel, IDatadogStats>();

        public static Func<string[], LogEventLevel, IDatadogStats> CreateCollector { get; set; } = (defaultTags, level) =>
        {
            var levelTag = $"level:{level.ToString().ToLowerInvariant()}";

            return new DatadogStats(
                address: "127.0.0.1",
                port: 8125, // Optional, default is 8125
                metricNamePrefix: null, // Optinal, if exists prefix, append "prefix." on every metrics call
                defaultTags: defaultTags.Concat(new[] {levelTag})
                    .ToArray() // Optional, append this tag with called tags
            );
        };

        protected override void OnAttached()
        {
            base.OnAttached();

            var tags = DefaultTags.Split('\n');

            foreach (var level in Enum.GetValues(typeof(LogEventLevel)).Cast<LogEventLevel>())
            {
                _collectors.Add(level,CreateCollector(tags, level));
            }
        }

        public void On(Event<LogEventData> evt)
        {
            var collector = _collectors[evt.Data.Level];
            collector.Increment(MetricName);
        }

        public void Dispose()
        {
            foreach(var collector in _collectors)
                (collector.Value as IDisposable)?.Dispose();
        }
    }
}