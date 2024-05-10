using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate.Events;
using Nova.Friend.Domain.UserAggregate.Repositories;

namespace Nova.Friend.Application.Events;

public class AcceptedFriendRequestDomainEventHandler
    : INotificationHandler<AcceptedFriendRequestDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionScope _scope;

    public AcceptedFriendRequestDomainEventHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _scope = scope.AddReadScope(DatabaseOptions.UserCollection).AddWriteScope(DatabaseOptions.UserCollection);
    }
    
    public async Task Handle(AcceptedFriendRequestDomainEvent notification, CancellationToken cancellationToken)
    {
        await _unitOfWork.StartTransaction(_scope, cancellationToken);

        // TODO: Think about handling it        
        var sender = await _userRepository.FindUserById(notification.SenderId, cancellationToken); 
        if (sender is null)
            return;

        var receiver = await _userRepository.FindUserById(notification.ReceiverId, cancellationToken);
        if (receiver is null)
            return;
        
        sender.AddFriend(receiver.Id);
        receiver.AddFriend(sender.Id);

        await _userRepository.Update(sender, cancellationToken);
        await _userRepository.Update(receiver, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}