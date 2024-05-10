using FluentValidation;

namespace Nova.Friend.Application.Commands.RejectFriendRequest;

public class RejectFriendCommandValidator
    : AbstractValidator<RejectFriendCommand>
{
    public RejectFriendCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId should be not empty")
            .NotEqual(x => x.ReceiverId).WithMessage("The user cannot reject a friend request to himself");
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId should be not empty");
    }
}