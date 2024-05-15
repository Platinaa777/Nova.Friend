using System.Text.Json.Serialization;

namespace DomainDrivenDesign.Abstractions;

public class Id : ValueObject, IEquatable<Id>
{
    public string Value { get; }

    [JsonConstructor]
    protected Id(Guid id)
    {
        Value = id.ToString().ToUpper();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(Id? other) =>
        base.Equals(other);

    public static Result<Id> Create(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return Result.Failure<Id>(IdError.IdentityError);
        }

        return new Id(guidId);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj) =>
        obj is Id id && id.Equals(this);
}

public class IdError : Error
{
    public static readonly Error IdentityError = new("Identity", "InvalidId");
    
    public IdError(string code, string message) : base(code, message)
    {
    }
}