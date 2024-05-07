using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.UserAggregate.ValueObjects;

public class LastName : ValueObject
{
    private const int MaxLength = 30;
    public string Value { get; }

    private LastName(string lastName)
    {
        Value = lastName;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<LastName> Create(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<LastName>(UserError.EmptyLastName);
        if (lastName.Length > 30)
            return Result.Failure<LastName>(UserError.TooLongLastName);

        return new LastName(lastName);
    }
}