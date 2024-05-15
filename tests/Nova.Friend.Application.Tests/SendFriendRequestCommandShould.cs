using DomainDrivenDesign.Abstractions;
using FluentAssertions;
using Moq;
using Nova.Friend.Application.Commands.SendFriendRequest;
using Nova.Friend.Application.Factories;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class SendFriendRequestCommandShould
{
    private SendFriendRequestCommandHandler _sut = null!;

    private readonly Mock<IFriendRequestRepository> _friendRequestMock = new();
    private readonly Mock<IFriendSearchRepository> _friendSearchRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IIdGeneratorFactory<Guid>> _idGeneratorFactoryMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    private readonly UserId _senderId = UserId.Create("3F2D3FA3-E566-4A5A-9B78-FB4EC1723FB7").Value;
    private readonly UserId _receiverId = UserId.Create("DF0C354A-256B-484F-8BB5-A62BF09309D7").Value;
    private readonly SendFriendRequestCommand _command;
    
    
    public SendFriendRequestCommandShould()
    {
        _command = new()
        {
            SenderId = _senderId.Value,
            ReceiverId = _receiverId.Value
        };

        _idGeneratorFactoryMock.Setup(x => x.GenerateId())
            .Returns(Guid.Parse("1774184B-DC28-4E85-818D-F09E363C6EF6"));
        
        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
    }

    [Fact]
    public async Task ReturnSuccess_WhenUserAreNotFriends_PendingFriendRequestDoesNotExistNow()
    {
        User sender = User.Create(_senderId.Value, "firstname", "lastname", new()).Value;

        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);
        _friendSearchRepositoryMock.Setup(x =>
                x.CheckExistFriendRequestBetweenUsers(_senderId, _receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _userRepositoryMock.Object,
            _idGeneratorFactoryMock.Object, _unitOfWorkMock.Object, _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ReturnSenderNotFoundError_WhenSenderDoesNotExist()
    {
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _userRepositoryMock.Object,
            _idGeneratorFactoryMock.Object, _unitOfWorkMock.Object, _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RelationShipError.SenderNotFound);
    }
    
    [Fact]
    public async Task ReturnAlreadyFriendsError_WhenUsersAreFriends()
    {
        User sender = User.Create(_senderId.Value, "firstname", "lastname", new() { _receiverId }).Value;
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);
        
        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _userRepositoryMock.Object,
            _idGeneratorFactoryMock.Object, _unitOfWorkMock.Object, _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RelationShipError.AlreadyFriends);
    }
    
    [Fact]
    public async Task ReturnFriendRequestAlreadyExistsError_WhenFriendRequestAreExist()
    {
        User sender = User.Create(_senderId.Value, "firstname", "lastname", new()).Value;
        _userRepositoryMock.Setup(x => x.FindUserById(_senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);
        _friendSearchRepositoryMock.Setup(x =>
                x.CheckExistFriendRequestBetweenUsers(_senderId, _receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _userRepositoryMock.Object,
            _idGeneratorFactoryMock.Object, _unitOfWorkMock.Object, _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FriendRequestError.AlreadyExists);
    }
}