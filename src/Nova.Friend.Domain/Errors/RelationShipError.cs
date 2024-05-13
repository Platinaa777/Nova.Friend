using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public abstract class RelationShipError : Error
{
    public static readonly Error InvalidSenderId = new("User.Identity", "Invalid SenderId");
    public static readonly Error InvalidReceiverId = new("User.Identity", "Invalid ReceiverId");
    public static readonly Error SenderNotFound = new("User.Storage", "Sender not found");
    public static readonly Error ReceiverNotFound = new("User.Storage", "Receiver not found");
    public static readonly Error AlreadyFriends = new("User.Friendship", "Current user is already in friends list");

    protected RelationShipError(string code, string message) : base(code, message)
    {
    }
}