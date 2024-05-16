using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Monitoring;

namespace Nova.Friend.Application.Commands.DeleteFriend;

public class DeleteFriendCommand : IRequest<Result>, IMetricsRequest
{
    private const string CounterName = "delete.friend";
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public void SuccessOperation(ApplicationMetrics metrics)
    {
        metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(true));
    }

    public void FailureOperation(ApplicationMetrics metrics)
    {
        metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(false));
    }
}