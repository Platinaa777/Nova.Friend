using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public abstract class FriendRequestError : Error
{
    public static readonly Error InvalidFriendRequestId = new("FriendRequest.Identity", "Invalid FriendRequest id");
    public static readonly Error NotExistingStatusType = new("FriendRequest.Status", "This type of status does not exists");
    public static readonly Error EmptyStatusType = new("FriendRequest.Status", "FriendRequest status must be not empty");
    public static readonly Error AlreadyExists = new("FriendRequest.Creation", "FriendRequest between current users is already existed");
    public static readonly Error NotFound = new("FriendRequest.Search", "FriendRequest was not found");
    public static readonly Error EmptyRequestsList = new("FriendRequest.Search", "User does not have any friend requests");

    protected FriendRequestError(string code, string message) : base(code, message)
    {
    }
}