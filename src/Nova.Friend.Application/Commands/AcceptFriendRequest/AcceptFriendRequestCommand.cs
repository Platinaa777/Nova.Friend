using DomainDrivenDesign.Abstractions;
using MediatR;

namespace Nova.Friend.Application.Commands.AcceptFriendRequest;

public class AcceptFriendRequestCommand : IRequest<Result>
{
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
}