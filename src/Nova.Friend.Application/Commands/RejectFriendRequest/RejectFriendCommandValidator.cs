using FluentValidation;

namespace Nova.Friend.Application.Commands.RejectFriendRequest;

public class RejectFriendCommandValidator
    : AbstractValidator<RejectFriendCommand>
{
    public RejectFriendCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId should be not empty");
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId should be not empty");
    }
}