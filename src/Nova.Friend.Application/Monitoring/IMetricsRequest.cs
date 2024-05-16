namespace Nova.Friend.Application.Monitoring;

public interface IMetricsRequest
{
    void SuccessOperation(ApplicationMetrics metrics);
    void FailureOperation(ApplicationMetrics metrics);
}