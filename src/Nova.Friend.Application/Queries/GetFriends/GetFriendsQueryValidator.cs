using FluentValidation;

namespace Nova.Friend.Application.Queries.GetFriends;

public class GetFriendsQueryValidator
    : AbstractValidator<GetFriendsQuery>
{
    public GetFriendsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must not be empty");
    }
}