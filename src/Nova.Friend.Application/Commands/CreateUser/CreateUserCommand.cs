using DomainDrivenDesign.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Monitoring;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.CreateUser;

public class CreateUserCommand : IRequest<Result>, IMetricsRequest
{
    private const string CounterName = "user.created";
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public void SuccessOperation(ApplicationMetrics metrics)
    {
       metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(true));
    }

    public void FailureOperation(ApplicationMetrics metrics)
    {
        metrics.IncrementCount(CounterName, 1, ApplicationMetrics.ResultTags(false));
    }
}

public static class CreateUserCommandExtensions
{
    public static Result<User> ToCreateUserResult(this CreateUserCommand command) =>
        User.Create(command.UserId, command.UserName, command.LastName, new List<Id>());
}