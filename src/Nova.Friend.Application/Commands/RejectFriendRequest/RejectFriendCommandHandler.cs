using DomainDrivenDesign.Abstractions;
using FluentValidation;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.RejectFriendRequest;

public class RejectFriendCommandHandler
    : AbstractValidator<RejectFriendCommand>
{
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IFriendSearchRepository _friendSearchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectFriendCommandHandler(
        IFriendRequestRepository friendRequestRepository,
        IFriendSearchRepository friendSearchRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _friendRequestRepository = friendRequestRepository;
        _friendSearchRepository = friendSearchRepository;
        _unitOfWork = unitOfWork;
        scope.AddReadScope(DatabaseOptions.RequestCollection).AddWriteScope(DatabaseOptions.RequestCollection);
    }
     
    public async Task<Result> Handle(RejectFriendCommand request, CancellationToken cancellationToken)
    {
        var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

        // TODO: make intercept between errors
        if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            return Result.Failure(senderIdResult.Error);

        await _unitOfWork.StartTransaction(cancellationToken);
        
        var existingFriendRequest =
            await _friendSearchRepository.FindBySenderAndReceiver(
                senderIdResult.Value,
                receiverIdResult.Value, 
                cancellationToken);
          
        if (existingFriendRequest is null)
            return Result.Failure(FriendRequestError.NotFound);
          
        existingFriendRequest.Reject();

        await _friendRequestRepository.Update(existingFriendRequest, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
