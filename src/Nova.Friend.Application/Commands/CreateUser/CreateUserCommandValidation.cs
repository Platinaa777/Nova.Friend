using FluentValidation;

namespace Nova.Friend.Application.Commands.CreateUser;

public class CreateUserCommandValidation
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must be not emtpty");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName must be not empty")
            .MaximumLength(30).WithMessage("Maximum length of UserName is 30 characters");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName must be not empty")
            .MaximumLength(30).WithMessage("Maximum length of LastName is 30 characters");
    }
}