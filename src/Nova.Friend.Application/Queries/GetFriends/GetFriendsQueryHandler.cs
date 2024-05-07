using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Models;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Queries.GetFriends;

public class GetFriendsQueryHandler
    : IRequestHandler<GetFriendsQuery, Result<List<FriendInfo>>>
{
    private readonly IUserRepository _userRepository;

    public GetFriendsQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<List<FriendInfo>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
            return Result.Failure<List<FriendInfo>>(userIdResult.Error);

        var existingUser = await _userRepository.FindUserById(userIdResult.Value, cancellationToken);
        if (existingUser is null)
            return Result.Failure<List<FriendInfo>>(UserError.NotFound);

        return Result.Success(existingUser.Friends
            .Select(f => new FriendInfo { Id = f.Value }).ToList());
    }
}