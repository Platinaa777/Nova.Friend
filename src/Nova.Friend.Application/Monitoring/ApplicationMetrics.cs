using System.Diagnostics.Metrics;

namespace Nova.Friend.Application.Monitoring;

public class ApplicationMetrics
{
    public ApplicationMetrics()
    {
        var meter = new Meter("Nova.Friend.Application");
    }
}