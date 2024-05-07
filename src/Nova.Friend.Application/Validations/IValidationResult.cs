using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Application.Validations;

public interface IValidationResult
{
    public static readonly Error DefaultValidationError = new Error(
        "ValidationError", "Validation error was thrown");
    List<Error> Errors { get; set; }
}