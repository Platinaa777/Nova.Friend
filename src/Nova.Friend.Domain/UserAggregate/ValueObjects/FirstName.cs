using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.UserAggregate.ValueObjects;

public class FirstName : ValueObject
{
    public string Value { get; }

    public FirstName(string firstName)
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
        if (firstName.Length > 30)
            return Result.Failure<FirstName>(UserError.TooLongFirstName);

        return new FirstName(firstName);
    }
}