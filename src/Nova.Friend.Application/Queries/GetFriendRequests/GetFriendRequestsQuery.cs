using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Models;
using Nova.Friend.Application.Monitoring;

namespace Nova.Friend.Application.Queries.GetFriendRequests;

public class GetFriendRequestsQuery : IRequest<Result<List<FriendRequestInfo>>>, IMetricsRequest
{
    private const string CounterName = "get.friends";
    public string UserId { get; set; } = string.Empty;
    public void SuccessOperation(ApplicationMetrics metrics)
    {
        metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(true));
    }

    public void FailureOperation(ApplicationMetrics metrics)
    {
        metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(false));
    }
}