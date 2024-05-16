using System.Diagnostics;
using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Monitoring;

namespace Nova.Friend.Application.Behavior;

public class MonitoringPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IMetricsRequest
    where TResponse : Result
{
    private readonly ApplicationMetrics _metrics;

    public MonitoringPipelineBehavior(ApplicationMetrics metrics)
    {
        _metrics = metrics;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse result;
        using var activity = ApplicationMetrics.ActivitySource.StartActivity(
            request.GetType().Name,
            ActivityKind.Internal,
            default(ActivityContext));
        activity?.AddTag("nova.request", request.GetType().Name);
        
        try
        {
            result = await next();

            if (result.IsSuccess)
            {
                activity?.AddTag("error", false);
                request.SuccessOperation(_metrics);
                return result;
            }
            request.FailureOperation(_metrics);
            activity?.AddTag("error", true);
        }
        catch
        {
            activity?.AddTag("error", true);
            request.FailureOperation(_metrics);
            throw;
        } 
        
        return result;
    }
}