namespace Nova.Friend.Domain.Exceptions.User;

public class FirstNameException : DomainException
{
    public FirstNameException(int firstNameLength)
        : base($"FirstName should be less than 30 characters, now it equals to {firstNameLength}")
    {
    }
}