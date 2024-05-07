using FluentValidation;

namespace Nova.Friend.Application.Commands.SendFriendRequest;

public class SendFriendRequestCommandValidator
    : AbstractValidator<SendFriendRequestCommand>
{
    public SendFriendRequestCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId must be not empty");
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId must be not empty");
    }
}