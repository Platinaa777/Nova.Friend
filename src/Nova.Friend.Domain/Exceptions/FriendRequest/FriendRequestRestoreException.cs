namespace Nova.Friend.Domain.Exceptions.FriendRequest;

public class FriendRequestRestoreException : DomainException
{
    public FriendRequestRestoreException(string message)
        : base($"Error while restoring friend request: {message}")
    {
    }
}