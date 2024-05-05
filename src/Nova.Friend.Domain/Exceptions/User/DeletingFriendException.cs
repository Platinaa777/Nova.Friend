namespace Nova.Friend.Domain.Exceptions.User;

public class DeletingFriendException : DomainException
{
    public DeletingFriendException(string userId, string friendId)
        : base($"User with id {userId} cant delete from friend list the user with id {friendId}")
    {
    }
}