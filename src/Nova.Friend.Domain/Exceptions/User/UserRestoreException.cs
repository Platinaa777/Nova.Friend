namespace Nova.Friend.Domain.Exceptions.User;

public class UserRestoreException : DomainException
{
    public UserRestoreException(string message)
        : base($"Error while restoring user: {message}")
    {
    }
}
