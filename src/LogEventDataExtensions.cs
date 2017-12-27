using Seq.Apps.LogEvents;

namespace Seq.App.Datadog
{
    public static class LogEventDataExtensions
    {
        public static string GetString(this LogEventData data, string propertyName)
        {
            if (data?.Properties == null)
                return null;

            if (!data.Properties.ContainsKey(propertyName))
                return null;

            var value = data.Properties[propertyName];

            return value?.ToString();
        }
    }
}