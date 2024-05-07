using FluentValidation;

namespace Nova.Friend.Application.Commands.DeleteFriend;

public class DeleteFriendCommandValidator
    : AbstractValidator<DeleteFriendCommand>
{
    public DeleteFriendCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId should be not empty");
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId should be not empty");
    }
}