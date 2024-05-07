using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public class RelationShipError : Error
{
    public static Error InvalidSenderId = new("User.Identity", "Invalid SenderId");
    public static Error InvalidReceiverId = new("User.Identity", "Invalid ReceiverId");
    public static Error SenderNotFound = new("User.Storage", "Sender not found");
    public static Error ReceiverNotFound = new("User.Storage", "Receiver not found");

    
    public RelationShipError(string code, string message) : base(code, message)
    {
    }
}