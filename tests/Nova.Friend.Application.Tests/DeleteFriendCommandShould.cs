using DomainDrivenDesign.Abstractions;
using FluentAssertions;
using Moq;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.Application.Commands.DeleteFriend;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Exceptions.User;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class DeleteFriendCommandShould
{
    private DeleteFriendCommandHandler _sut = null!;

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IFriendRepository> _friendRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    private readonly UserId _senderId;
    private readonly UserId _receiverId;
    private readonly DeleteFriendCommand _command;
    private User _sender;
    private User _receiver;

    public DeleteFriendCommandShould()
    {
        _senderId = UserId.Create("E099E1F0-7DEC-4677-B921-B76EF836CD38").Value;
        _receiverId = UserId.Create("B01EF0BE-60AF-48FD-B31B-37B99FACC2C5").Value;
        _command = new()
        {
            SenderId = _senderId.Value,
            ReceiverId = _receiverId.Value
        };
        
        _sender = User.Create(_senderId.Value, "senderName", "senderLastName", new() { _receiverId }).Value;
        _receiver = User.Create(_receiverId.Value, "receiverName", "receiverLastName", new() { _senderId }).Value;
        
        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
    }

    [Fact]
    public async Task ReturnSuccess_WhenFriendsWasDeleted()
    {
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_sender);
        _userRepositoryMock.Setup(x => x.FindUserById(_receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_receiver);
        
        _sut = new(_userRepositoryMock.Object, _friendRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);
        
        Thread.Sleep(500);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ThrowException_WhenFriendsNotExistToEachOther()
    {
        _sender = User.Create(_senderId.Value, "senderName", "senderLastName", new() ).Value;
        _receiver = User.Create(_receiverId.Value, "receiverName", "receiverLastName", new()).Value;
        
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_sender);
        _userRepositoryMock.Setup(x => x.FindUserById(_receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_receiver);
        
        _sut = new(_userRepositoryMock.Object, _friendRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        await _sut.Invoking(x => x.Handle(_command, CancellationToken.None))
            .Should().ThrowAsync<DeletingFriendException>();
    }
}