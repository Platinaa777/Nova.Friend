using FluentValidation;

namespace Nova.Friend.Application.Commands.AcceptFriendRequest;

public class AcceptFriendRequestCommandValidator
    : AbstractValidator<AcceptFriendRequestCommand>
{
    public AcceptFriendRequestCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId should be not empty");
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId should be not empty");
    }
}