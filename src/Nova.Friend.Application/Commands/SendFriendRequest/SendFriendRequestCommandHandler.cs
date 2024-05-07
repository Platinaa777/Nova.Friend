using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Factories;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendShipInvitationAggregate;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.SendFriendRequest;

public class SendFriendRequestCommandHandler
    : IRequestHandler<SendFriendRequestCommand, Result>
{
    private readonly IFriendRequestRepository _friendRepository;
    private readonly IFriendSearchRepository _friendSearchRepository;
    private readonly IIdGeneratorFactory<Guid> _idFactory;

    public SendFriendRequestCommandHandler(
        IFriendRequestRepository friendRepository,
        IFriendSearchRepository friendSearchRepository,
        IIdGeneratorFactory<Guid> idFactory)
    {
        _friendRepository = friendRepository;
        _friendSearchRepository = friendSearchRepository;
        _idFactory = idFactory;
    }
    
    public async Task<Result> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

        // TODO: make intercept between errors
        if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            return Result.Failure(senderIdResult.Error);

        var friendRequestResult = request.ToFriendRequestResult(_idFactory.GenerateId());
        if (friendRequestResult.IsFailure)
            return Result.Failure(friendRequestResult.Error);

        var existingRequest = 
            await _friendSearchRepository.FindFriendRequestBetweenUsers(senderIdResult.Value, receiverIdResult.Value);

        if (existingRequest)
            return Result.Failure(FriendRequestError.AlreadyExists);


        await _friendRepository.Add(friendRequestResult.Value, cancellationToken);

        return Result.Success();
    }
}