namespace Nova.Friend.Domain.Tests;

public class FriendRequestTests
{
    [Fact]
    public void HandleCreateInvitation_WhenAllParametersValid_ShouldBeSuccess()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void HandleCreateInvitation_WhenSenderIdInvalid_ShouldBeFailure()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "invalid",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void HandleCreateInvitation_WhenInvitationIdInvalid_ShouldBeFailure()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "invalid",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void HandleRejectInvitation_WhenInPendingStatus_ShouldBeRejected()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeTrue();
        
        friendShipInvitationResult.Value.Reject();

        friendShipInvitationResult.Value.Status.Should()
            .BeEquivalentTo(RequestStatus.Rejected);
        friendShipInvitationResult.Value.GetDomainEvents().Should().BeEmpty();
    }
    
    [Fact]
    public void HandleRejectInvitation_WhenNotInPendingStatus_ShouldBeThrowException()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Rejected.Name).Value;

        friendShipInvitationResult.Invoking(f => f.Reject())
            .Should().Throw<RejectedFriendRequestException>();
    }
    
    [Fact]
    public void HandleAcceptInvitation_WhenInPendingStatus_ShouldBeAccepted()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeTrue();
        
        friendShipInvitationResult.Value.Accept();

        friendShipInvitationResult.Value.Status.Should()
            .BeEquivalentTo(RequestStatus.Accepted);
        friendShipInvitationResult.Value.GetDomainEvents().Should()
            .ContainSingle(x => x.GetType() == typeof(AcceptedFriendRequestDomainEvent));
    }
    
    [Fact]
    public void HandleAcceptInvitation_WhenNotInPendingStatus_ShouldBeThrowException()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Rejected.Name).Value;

        friendShipInvitationResult.Invoking(f => f.Accept())
            .Should().Throw<AcceptedInvitationException>();
    }
    
    [Fact]
    public void HandleCancelInvitation_WhenInPendingStatus_ShouldBeCanceled()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Pending.Name);

        friendShipInvitationResult.IsSuccess.Should().BeTrue();
        
        friendShipInvitationResult.Value.Cancel();

        friendShipInvitationResult.Value.Status.Should()
            .BeEquivalentTo(RequestStatus.Canceled);
        friendShipInvitationResult.Value.GetDomainEvents().Should().BeEmpty();
    }
    
    [Fact]
    public void HandleCancelInvitation_WhenNotInPendingStatus_ShouldBeThrowException()
    {
        var friendShipInvitationResult = FriendRequest.Create(
            "5FDF90DA-AFB3-4F91-A974-94AFEF594E88",
            "F5284E8F-3358-4D9E-82CA-A784FB243703",
            "AF4D0068-CE9F-4418-98A5-01BE7B41B544",
            RequestStatus.Rejected.Name).Value;

        friendShipInvitationResult.Invoking(f => f.Cancel())
            .Should().Throw<CanceledFriendRequestException>();
    }
}