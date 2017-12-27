using System.Threading.Tasks;

namespace Seq.App.Datadog
{
    public interface IDatadog
    {
        Task SendMetricAsync(string metricName, decimal value);
    }
}