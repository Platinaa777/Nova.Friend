using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Exceptions.User;
using Nova.Friend.Domain.UserAggregate.Events;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.UserAggregate;

public class User : AggregateRoot<UserId>
{
    private User(
        UserId id,
        FirstName firstName,
        LastName lastName,
        HashSet<UserId> friends)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Friends = friends;
    }
    
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public HashSet<UserId> Friends { get; }

    public void AddFriend(UserId userId)
    {
        Friends.Add(userId);
    }

    public void DeleteFromFriends(UserId friendId)
    {
        if (!Friends.Contains(friendId))
            throw new DeletingFriendException(Id.Value, friendId.Value);

        Friends.Remove(friendId);
        
        RaiseDomainEvent(new RemovedFriendDomainEvent(Id, friendId));
    } 

    public void ChangeFirstName(string firstName)
    {
        var result = FirstName.Create(firstName);
        if (result.IsFailure)
            throw new FirstNameException(firstName.Length);

        FirstName = result.Value;
    }

    public void ChangeLastName(string lastName)
    {
        var result = LastName.Create(lastName);
        if (result.IsFailure)
            throw new LastNameException(lastName.Length);

        LastName = result.Value;
    }

    public static Result<User> Create(
        string userId,
        string firstName,
        string lastName,
        List<UserId> friends)
    {
        var userIdResult = UserId.Create(userId);
        if (userIdResult.IsFailure)
            return Result.Failure<User>(userIdResult.Error);

        var firstNameResult = FirstName.Create(firstName);
        if (firstNameResult.IsFailure)
            return Result.Failure<User>(firstNameResult.Error);

        var lastNameResult = LastName.Create(lastName);
        if (lastNameResult.IsFailure)
            return Result.Failure<User>(lastNameResult.Error);

        var friendList = friends.ToHashSet();

        return new User(userIdResult.Value, firstNameResult.Value, lastNameResult.Value, friendList);
    }
}