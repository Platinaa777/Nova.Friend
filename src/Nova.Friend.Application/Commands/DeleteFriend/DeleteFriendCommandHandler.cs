using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.DeleteFriend;

public class DeleteFriendCommandHandler
    : IRequestHandler<DeleteFriendCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteFriendCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
    {
        var result = await FindUsers(request.SenderId, request.ReceiverId);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        var (sender, receiver) = result.Value;
        
        sender.DeleteFromFriends(receiver.Id);        
        receiver.DeleteFromFriends(sender.Id);

        return Result.Success();
    }

    private async Task<Result<(User sender, User receiver)>> FindUsers(string senderId, string receiverId)
    {
        var senderIdResult = UserId.Create(senderId);
        if (senderIdResult.IsFailure)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.InvalidSenderId);
        
        var receiverIdResult = UserId.Create(receiverId);
        if (receiverIdResult.IsFailure)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.InvalidReceiverId);

        var sender = await _userRepository.FindUserById(senderIdResult.Value);
        if (sender is null)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.SenderNotFound);
        
        var receiver = await _userRepository.FindUserById(senderIdResult.Value);
        if (receiver is null)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.ReceiverNotFound);

        return (sender, receiver);
    }
}