using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Models;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler
    : IRequestHandler<GetFriendRequestsQuery, Result<List<FriendRequestInfo>>>
{
    private readonly IFriendSearchRepository _searchRepository;

    public GetFriendRequestsQueryHandler(
        IFriendSearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }
    
    public async Task<Result<List<FriendRequestInfo>>> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
            return Result.Failure<List<FriendRequestInfo>>(userIdResult.Error);

        var result = await _searchRepository.GetFriendRequests(userIdResult.Value, cancellationToken);
        if (result is null || result.Count == 0)
            return Result.Success(new List<FriendRequestInfo>());

        List<FriendRequestInfo> response = new();
        foreach (var fr in result)
        {
            response.Add(new FriendRequestInfo
            {
                SenderId = fr.SenderId.Value
            });
        }

        return response;
    }
}