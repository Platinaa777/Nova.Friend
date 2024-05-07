using DomainDrivenDesign.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.CreateUser;

public class CreateUserCommand : IRequest<Result>
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public static class CreateUserCommandExtensions
{
    public static Result<User> ToCreateUserResult(this CreateUserCommand command) =>
        User.Create(command.UserId, command.UserName, command.LastName, new List<UserId>());
}