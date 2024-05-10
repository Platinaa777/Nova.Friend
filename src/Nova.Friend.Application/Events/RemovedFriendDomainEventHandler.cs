using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.UserAggregate.Events;
using Nova.Friend.Domain.UserAggregate.Repositories;

namespace Nova.Friend.Application.Events;

public class RemovedFriendDomainEventHandler
    : INotificationHandler<RemovedFriendDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionScope _scope;

    public RemovedFriendDomainEventHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _scope = scope.AddReadScope(DatabaseOptions.UserCollection).AddWriteScope(DatabaseOptions.UserCollection);
    }
    
    public async Task Handle(RemovedFriendDomainEvent notification, CancellationToken cancellationToken)
    {
        await _unitOfWork.StartTransaction(_scope, cancellationToken);

        // TODO: Think about handling it        
        var initiator = await _userRepository.FindUserById(notification.FriendRemovalInitiator, cancellationToken); 
        if (initiator is null)
            return;

        var removedFriend = await _userRepository.FindUserById(notification.RemovedFriend, cancellationToken);
        if (removedFriend is null)
            return;
        
        initiator.DeleteFromFriends(removedFriend.Id);
        removedFriend.DeleteFromFriends(initiator.Id);

        await _userRepository.Update(initiator, cancellationToken);
        await _userRepository.Update(removedFriend, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}