using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Models;

namespace Nova.Friend.Application.Queries.GetFriends;

public class GetFriendsQuery : IRequest<Result<List<FriendInfo>>>
{
    public string UserId { get; init; } = string.Empty;
}