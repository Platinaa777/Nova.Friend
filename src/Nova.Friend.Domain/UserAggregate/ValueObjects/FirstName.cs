using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.UserAggregate.ValueObjects;

public class FirstName : ValueObject
{
    private const int MaxLength = 30;
    public string Value { get; }

    private FirstName(string firstName)
    {
        Value = firstName;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<FirstName> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<FirstName>(UserError.EmptyFirstName);
        if (firstName.Length > MaxLength)
            return Result.Failure<FirstName>(UserError.TooLongFirstName);

        return new FirstName(firstName);
    }
}