using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public class UserError : Error
{
    public static Error InvalidUserId = new("User.Identity.Error", "Invalid user id");
    public static Error EmptyLastName = new("User.Data", "Empty LastName of user");
    public static Error TooLongLastName = new("User.Data", "LastName should be less than 30 characters long");
    public static Error EmptyFirstName = new("User.Data", "Empty FirstName of user");
    public static Error TooLongFirstName = new("User.Data", "FirstName should be less than 30 characters long");
    public static Error NotFound = new("User.Storage", "User not found");
    public static Error AlreadyExists = new("User.Storage", "User is already exists");
    
    
    public UserError(string code, string message) : base(code, message)
    {
    }
}