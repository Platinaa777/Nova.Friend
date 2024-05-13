using DomainDrivenDesign.Abstractions;
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
    private readonly IFriendRepository _friendRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptedFriendRequestDomainEventHandler(
        IUserRepository userRepository,
        IFriendRepository friendRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _userRepository = userRepository;
        _friendRepository = friendRepository;
        _unitOfWork = unitOfWork;
        scope
            .AddReadScope(DatabaseOptions.UserCollection)
            .AddWriteScope(DatabaseOptions.UserCollection)
            .AddWriteScope(DatabaseOptions.FriendEdge);
    }
    
    public async Task Handle(AcceptedFriendRequestDomainEvent notification, CancellationToken cancellationToken)
    {
        await _unitOfWork.StartTransaction(cancellationToken);
        var (_, senderId, receiverId) = CreateIds(notification);

        // TODO: Think about handling it        
        var sender = await _userRepository.FindUserById(senderId, cancellationToken); 
        if (sender is null)
            return;

        var receiver = await _userRepository.FindUserById(receiverId, cancellationToken);
        if (receiver is null)
            return;
        
        sender.AddFriend(receiver.Id);
        receiver.AddFriend(sender.Id);

        await _friendRepository.AddFriend(senderId, receiverId);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private (Id requestId, Id senderId, Id receiverId) CreateIds(AcceptedFriendRequestDomainEvent notification) =>
        (Id.Create(notification.RequestId).Value, Id.Create(notification.SenderId).Value, Id.Create(notification.ReceiverId).Value);
}