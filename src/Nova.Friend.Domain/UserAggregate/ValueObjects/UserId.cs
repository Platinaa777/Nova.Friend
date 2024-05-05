using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.UserAggregate.ValueObjects;

public class UserId : ValueObject, IEquatable<UserId>
{
    public string Value { get; }

    private UserId(Guid userId)
    {
        Value = userId.ToString();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(UserId? other) => other is not null && other.Equals(this);

    public static Result<UserId> Create(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return Result.Failure<UserId>(UserError.InvalidUserId);
        }

        return new UserId(id);
    }
}