using DomainDrivenDesign.Abstractions;
using FluentAssertions;
using Moq;
using Nova.Friend.Application.Commands.RejectFriendRequest;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.Exceptions.FriendRequest;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Enumerations;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class RejectFriendCommandShould
{
    private RejectFriendCommandHandler _sut = null!;

    private readonly Mock<IFriendRequestRepository> _friendRequestMock = new();
    private readonly Mock<IFriendSearchRepository> _friendSearchRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    private readonly UserId _senderId = UserId.Create("3F2D3FA3-E566-4A5A-9B78-FB4EC1723FB7").Value;
    private readonly UserId _receiverId = UserId.Create("DF0C354A-256B-484F-8BB5-A62BF09309D7").Value;
    private readonly RejectFriendCommand _command;
    private FriendRequest _friendRequest;
    
    
    public RejectFriendCommandShould()
    {
        _command = new()
        {
            SenderId = _senderId.Value,
            ReceiverId = _receiverId.Value
        };

        _friendRequest = FriendRequest.Create("FF3A978A-F0C1-45EB-AF28-1F0E6FD73AD4", _senderId.Value,
            _receiverId.Value, RequestStatus.Pending.Name).Value;
        
        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
    }

    [Fact]
    public async Task ReturnSuccess_WhenRequestInPendingStatus()
    {
        _friendSearchRepositoryMock.Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, CancellationToken.None))
            .ReturnsAsync(_friendRequest);
        
        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ThrowException_WhenRequestInNotPendingStatus()
    {
        _friendRequest = FriendRequest.Create("FF3A978A-F0C1-45EB-AF28-1F0E6FD73AD4", _senderId.Value,
            _receiverId.Value, RequestStatus.Accepted.Name).Value;
        
        _friendSearchRepositoryMock.Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, CancellationToken.None))
            .ReturnsAsync(_friendRequest);
        
        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        await _sut.Invoking(x => x.Handle(_command, CancellationToken.None))
            .Should().ThrowAsync<RejectedFriendRequestException>();
    }

    [Fact]
    public async Task ReturnFriendRequestNotFoundError_WhenFriendRequestDoesNotExist()
    {
        _friendSearchRepositoryMock.Setup(x => x.FindBySenderAndReceiver(_senderId, _receiverId, CancellationToken.None))
            .ReturnsAsync((FriendRequest?)null);

        _sut = new(_friendRequestMock.Object, _friendSearchRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FriendRequestError.NotFound);
    }
}