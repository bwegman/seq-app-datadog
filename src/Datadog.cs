using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Seq.App.Datadog
{
    public class Datadog : IDatadog
    {
        private static readonly string HostName;

        private const string SeriesUrl = "https://app.datadoghq.com/api/v1/series";

        private readonly string _metricPrefix;
        private readonly string _apiKey;

        static Datadog()
        {
            HostName = Dns.GetHostName();
        }

        public Datadog(string metricPrefix, string apiKey)
        {
            _metricPrefix = metricPrefix;
            _apiKey = apiKey;
        }

        public async Task SendMetricAsync(string metricName, decimal value)
        {
            var now = ToUnixTime(DateTime.UtcNow);

            var client = new HttpClient();

            var response = await client.PostAsJsonAsync(
                $"{SeriesUrl}?api_key={_apiKey}",
                    new RootObject
                    {
                        series = new[] {
                            new Series
                            {
                                metric = $"{_metricPrefix}{metricName}",
                                points = new[]
                                {
                                    new [] { now, value }
                                },
                                type = "gauge",
                                host = HostName,
                                tags = new [] { "..." }
                            }
                        }
                    }
                );

            response.EnsureSuccessStatusCode();
        }

        private static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
    }

    public class RootObject
    {
        public Series[] series { get; set; }
    }

    public class Series
    {
        public string metric { get; set; }
        public decimal[][] points { get; set; }
        public string type { get; set; }
        public string host { get; set; }
        public string[] tags { get; set; }
    }
}
