using DomainDrivenDesign.Abstractions;
using FluentValidation;
using Nova.Friend.Application.Commands.AcceptFriendRequest;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.RejectFriendRequest;

public class RejectFriendCommandHandler
    : AbstractValidator<RejectFriendCommand>
{
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IFriendSearchRepository _friendSearchRepository;

    public RejectFriendCommandHandler(
        IFriendRequestRepository friendRequestRepository,
        IFriendSearchRepository friendSearchRepository)
    {
        _friendRequestRepository = friendRequestRepository;
        _friendSearchRepository = friendSearchRepository;
    }
     
    public async Task<Result> Handle(RejectFriendCommand request, CancellationToken cancellationToken)
    {
        var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

        // TODO: make intercept between errors
        if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            return Result.Failure(senderIdResult.Error);

        var existingFriendRequest =
            await _friendSearchRepository.FindBySenderAndReceiver(
                senderIdResult.Value,
                receiverIdResult.Value);
          
        if (existingFriendRequest is null)
            return Result.Failure(FriendRequestError.NotFound);
          
        existingFriendRequest.Reject();

        await _friendRequestRepository.Update(existingFriendRequest, cancellationToken);

        return Result.Success();
    }
}
