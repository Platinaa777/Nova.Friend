using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public class FriendRequestError : Error
{
    public static Error InvalidFriendRequestId = new("FriendRequest.Identity", "Invalid FriendRequest id");
    public static Error NotExistingStatusType = new("FriendRequest.Status", "This type of status does not exists");
    public static Error EmptyStatusType = new("FriendRequest.Status", "FriendRequest status must be not empty");
    public static Error AlreadyExists = new("FriendRequest.Creation", "FriendRequest between current users is already existed");
    public static Error NotFound = new("FriendRequest.Search", "FriendRequest was not found");

    public FriendRequestError(string code, string message) : base(code, message)
    {
    }
}