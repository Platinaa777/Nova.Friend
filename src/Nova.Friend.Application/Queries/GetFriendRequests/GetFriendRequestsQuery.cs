using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Models;

namespace Nova.Friend.Application.Queries.GetFriendRequests;

public class GetFriendRequestsQuery : IRequest<Result<List<FriendRequestInfo>>>
{
    public string UserId { get; set; } = string.Empty;
}