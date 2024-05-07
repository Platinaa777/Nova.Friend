using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Domain.FriendShipInvitationAggregate;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Enumerations;

namespace Nova.Friend.Application.Commands.SendFriendRequest;

public class SendFriendRequestCommand : IRequest<Result>
{
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
}

public static class SendFriendRequestCommandExtensions
{
    public static Result<FriendRequest> ToFriendRequestResult(this SendFriendRequestCommand command, Guid id) =>
        FriendRequest.Create(id.ToString(), command.SenderId, command.ReceiverId, RequestStatus.Pending.Name);
}