using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public abstract class UserError : Error
{
    public static readonly Error InvalidUserId = new("User.Identity.Error", "Invalid user id");
    public static readonly Error EmptyLastName = new("User.Data", "Empty LastName of user");
    public static readonly Error TooLongLastName = new("User.Data", "LastName should be less than 30 characters long");
    public static readonly Error EmptyFirstName = new("User.Data", "Empty FirstName of user");
    public static readonly Error TooLongFirstName = new("User.Data", "FirstName should be less than 30 characters long");
    public static readonly Error NotFound = new("User.Storage", "User not found");
    public static readonly Error AlreadyExists = new("User.Storage", "User is already exists");


    protected UserError(string code, string message) : base(code, message)
    {
    }
}