namespace Nova.Friend.Domain.Exceptions.User;

public class LastNameException : DomainException
{
    public LastNameException(int lastNameLength)
        : base($"FirstName should be less than 30 characters, now it equals to {lastNameLength}")
    {
    }
}