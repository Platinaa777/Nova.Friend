using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Commands.AcceptFriendRequest;

public class AcceptFriendRequestCommandHandler
     : IRequestHandler<AcceptFriendRequestCommand, Result>
{
     private readonly IFriendRequestRepository _friendRequestRepository;
     private readonly IFriendSearchRepository _friendSearchRepository;
     private readonly IUnitOfWork _unitOfWork;
     private readonly ITransactionScope _scope;

     public AcceptFriendRequestCommandHandler(
          IFriendRequestRepository friendRequestRepository,
          IFriendSearchRepository friendSearchRepository,
          IUnitOfWork unitOfWork,
          ITransactionScope scope)
     {
          _friendRequestRepository = friendRequestRepository;
          _friendSearchRepository = friendSearchRepository;
          _unitOfWork = unitOfWork;
          _scope = scope
               .AddReadScope(DatabaseOptions.RequestCollection)
               .AddWriteScope(DatabaseOptions.OutboxMessage)
               .AddWriteScope(DatabaseOptions.RequestCollection);
     }
     
     public async Task<Result> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
     {
          await _unitOfWork.StartTransaction(cancellationToken);
          
          var (senderIdResult, receiverIdResult) = (UserId.Create(request.SenderId), UserId.Create(request.ReceiverId));

          // TODO: make intercept between errors
          if (senderIdResult.IsFailure || receiverIdResult.IsFailure)
               return Result.Failure(senderIdResult.Error);

          var existingFriendRequest =
               await _friendSearchRepository.FindBySenderAndReceiver(
                    senderIdResult.Value,
                    receiverIdResult.Value,
                    cancellationToken);
          
          if (existingFriendRequest is null)
               return Result.Failure(FriendRequestError.NotFound);
          
          existingFriendRequest.Accept();

          await _friendRequestRepository.Update(existingFriendRequest, cancellationToken);

          await _unitOfWork.SaveChangesAsync(cancellationToken);

          return Result.Success();
     }
}