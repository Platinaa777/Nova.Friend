using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;
using IUnitOfWork = Nova.Friend.Application.TransactionScope.IUnitOfWork;

namespace Nova.Friend.Application.Commands.DeleteFriend;

public class DeleteFriendCommandHandler
    : IRequestHandler<DeleteFriendCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionScope _scope;

    public DeleteFriendCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _scope = scope.AddReadScope(DatabaseOptions.UserCollection).AddWriteScope(DatabaseOptions.UserCollection);
    }
    
    public async Task<Result> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
    {
        var result = await FindUsers(request.SenderId, request.ReceiverId, cancellationToken);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        var (sender, receiver) = result.Value;
        
        sender.DeleteFromFriends(receiver.Id);        
        receiver.DeleteFromFriends(sender.Id);

        await _userRepository.Update(sender, cancellationToken);
        await _userRepository.Update(receiver, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<(User sender, User receiver)>> FindUsers(string senderId, string receiverId, CancellationToken cancellationToken)
    {
        var senderIdResult = UserId.Create(senderId);
        if (senderIdResult.IsFailure)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.InvalidSenderId);
        
        var receiverIdResult = UserId.Create(receiverId);
        if (receiverIdResult.IsFailure)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.InvalidReceiverId);
        
        await _unitOfWork.StartTransaction(_scope, cancellationToken);

        var sender = await _userRepository.FindUserById(senderIdResult.Value, cancellationToken);
        if (sender is null)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.SenderNotFound);
        
        var receiver = await _userRepository.FindUserById(receiverIdResult.Value, cancellationToken);
        if (receiver is null)
            return Result.Failure<(User sender, User receiver)>(RelationShipError.ReceiverNotFound);

        return (sender, receiver);
    }
}