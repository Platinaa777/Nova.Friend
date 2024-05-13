using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.Factories;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.SendFriendRequest;

public class SendFriendRequestCommandHandler
    : IRequestHandler<SendFriendRequestCommand, Result>
{
    private readonly IFriendRequestRepository _friendRepository;
    private readonly IFriendSearchRepository _friendSearchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIdGeneratorFactory<Guid> _idFactory;
    private readonly IUnitOfWork _unitOfWork;

    public SendFriendRequestCommandHandler(
        IFriendRequestRepository friendRepository,
        IFriendSearchRepository friendSearchRepository,
        IUserRepository userRepository,
        IIdGeneratorFactory<Guid> idFactory,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _friendRepository = friendRepository;
        _friendSearchRepository = friendSearchRepository;
        _userRepository = userRepository;
        _idFactory = idFactory;
        _unitOfWork = unitOfWork;
        scope
            .AddReadScope(DatabaseOptions.RequestCollection)
            .AddReadScope(DatabaseOptions.UserCollection)
            .AddWriteScope(DatabaseOptions.RequestCollection);
    }
    
    public async Task<Result> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

        // TODO: make intercept between errors
        if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            return Result.Failure(senderIdResult.Error);

        await _unitOfWork.StartTransaction(cancellationToken);

        var friendsResult = await CheckAlreadyFriends(senderIdResult.Value, receiverIdResult.Value, cancellationToken);
        if (friendsResult.IsFailure)
            return Result.Failure(friendsResult.Error);

        var friendRequestResult = request.ToFriendRequestResult(_idFactory.GenerateId());
        if (friendRequestResult.IsFailure)
            return Result.Failure(friendRequestResult.Error);

        var existingRequest = 
            await _friendSearchRepository.CheckExistFriendRequestBetweenUsers(senderIdResult.Value, receiverIdResult.Value, cancellationToken);

        if (existingRequest)
            return Result.Failure(FriendRequestError.AlreadyExists);

        await _friendRepository.Add(friendRequestResult.Value, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> CheckAlreadyFriends(UserId senderId, UserId receiverId, CancellationToken cancellationToken)
    {
        var sender = await _userRepository.FindUserById(senderId, cancellationToken);
        
        if (sender is null)
            return Result.Failure(RelationShipError.SenderNotFound);

        if (sender.HasFriend(receiverId))
            return Result.Failure(RelationShipError.AlreadyFriends);

        return Result.Success();
    }
}