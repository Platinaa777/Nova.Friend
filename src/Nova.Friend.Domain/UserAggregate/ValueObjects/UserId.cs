using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.UserAggregate.ValueObjects;

public class UserId : Id, IEquatable<UserId>
{
    private UserId(Guid userId) : base(userId)
    {
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(UserId? other) =>
        base.Equals(other);

    public new static Result<UserId> Create(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return Result.Failure<UserId>(UserError.InvalidUserId);
        }

        return new UserId(id);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj) =>
        obj is UserId userId && userId.Equals(this);
}