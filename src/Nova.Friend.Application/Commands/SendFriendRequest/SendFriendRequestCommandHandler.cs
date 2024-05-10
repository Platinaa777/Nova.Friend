using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.Factories;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;
using IUnitOfWork = Nova.Friend.Application.TransactionScope.IUnitOfWork;

namespace Nova.Friend.Application.Commands.SendFriendRequest;

public class SendFriendRequestCommandHandler
    : IRequestHandler<SendFriendRequestCommand, Result>
{
    private readonly IFriendRequestRepository _friendRepository;
    private readonly IFriendSearchRepository _friendSearchRepository;
    private readonly IIdGeneratorFactory<Guid> _idFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionScope _scope;

    public SendFriendRequestCommandHandler(
        IFriendRequestRepository friendRepository,
        IFriendSearchRepository friendSearchRepository,
        IIdGeneratorFactory<Guid> idFactory,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _friendRepository = friendRepository;
        _friendSearchRepository = friendSearchRepository;
        _idFactory = idFactory;
        _unitOfWork = unitOfWork;
        _scope = scope.AddReadScope(DatabaseOptions.RequestCollection).AddWriteScope(DatabaseOptions.RequestCollection);
    }
    
    public async Task<Result> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

        // TODO: make intercept between errors
        if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
            return Result.Failure(senderIdResult.Error);

        await _unitOfWork.StartTransaction(_scope, cancellationToken);

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
}