using DomainDrivenDesign.Abstractions;
using FluentAssertions;
using Moq;
using Nova.Friend.Application.Commands.AcceptFriendRequest;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Exceptions.FriendRequest;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Enumerations;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class AcceptFriendCommandShould
{
    private AcceptFriendRequestCommandHandler _sut = null!;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IFriendRequestRepository> _friendRepositoryMock = new();
    private readonly Mock<IFriendSearchRepository> _friendSearchRepositoryMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();

    private readonly UserId _senderId = UserId.Create("3F2D3FA3-E566-4A5A-9B78-FB4EC1723FB7").Value;
    private readonly UserId _receiverId = UserId.Create("DF0C354A-256B-484F-8BB5-A62BF09309D7").Value;
    private readonly AcceptFriendRequestCommand _command;

    public AcceptFriendCommandShould()
    {
        _unitOfWorkMock.Setup(x => x.StartTransaction(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        
        _command = new AcceptFriendRequestCommand()
        {
            SenderId = _senderId.Value, 
            ReceiverId = _receiverId.Value
        };
    }
    
    [Fact]
    public async Task ReturnSuccess_WhenFriendRequestBetweenUsersInPendingStatus()
    {
        _friendSearchRepositoryMock
            .Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FriendRequest.Create("FB5F18BE-A24D-4C20-910B-86FF8C407008", _senderId.Value, _receiverId.Value,
                RequestStatus.Pending.Name).Value);
        
        _sut = new(_friendRepositoryMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);
        
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReturnNotFoundError_WhenFriendRequestBetweenUsersNotExist()
    {
        _friendSearchRepositoryMock
            .Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FriendRequest?)null);
        
        _sut = new(_friendRepositoryMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public async Task ThrowException_WhenFriendRequestBetweenUsersInNotPendingStatus()
    {
        _friendSearchRepositoryMock
            .Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FriendRequest.Create("FB5F18BE-A24D-4C20-910B-86FF8C407008", _senderId.Value, _receiverId.Value,
                RequestStatus.Accepted.Name).Value);
        
        _sut = new(_friendRepositoryMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        await _sut.Invoking(x => x.Handle(_command, It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<AcceptedInvitationException>();
    }
}