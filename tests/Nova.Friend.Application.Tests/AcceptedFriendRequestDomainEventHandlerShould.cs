using DomainDrivenDesign.Abstractions;
using Moq;
using Nova.Friend.Application.Events;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate.Events;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class AcceptedFriendRequestDomainEventHandlerShould
{
    private AcceptedFriendRequestDomainEventHandler _sut = null!;

    private readonly Mock<IFriendRepository> _friendRepoMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    private readonly UserId _senderId = UserId.Create("3F2D3FA3-E566-4A5A-9B78-FB4EC1723FB7").Value;
    private readonly UserId _receiverId = UserId.Create("DF0C354A-256B-484F-8BB5-A62BF09309D7").Value;
    private readonly AcceptedFriendRequestDomainEvent _command;
    private readonly User _sender;
    private readonly User _receiver;
    
    public AcceptedFriendRequestDomainEventHandlerShould()
    {
        _command = new("ADD95E64-7579-4BCE-AE2B-60365483ECBD", _senderId.Value, _receiverId.Value);
        
        _sender = User.Create(_senderId.Value, "senderName", "senderLastName", new()).Value;
        _receiver = User.Create(_receiverId.Value, "receiverName", "receiverLastName", new()).Value;

        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
    }

    [Fact]
    public async Task ReturnSuccess_WhenUsersAreNotFriends()
    {
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_sender);
        _userRepositoryMock.Setup(x => x.FindUserById(_receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_receiver);

        _sut = new(_userRepositoryMock.Object, _friendRepoMock.Object, _unitOfWorkMock.Object, _transactionScopeMock.Object);

        await _sut.Handle(_command, CancellationToken.None);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}