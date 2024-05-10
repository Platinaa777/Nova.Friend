using AutoMapper;
using Nova.Friend.Application.Commands.AcceptFriendRequest;
using Nova.Friend.Application.Commands.RejectFriendRequest;
using Nova.Friend.Application.Commands.SendFriendRequest;
using Nova.Friend.Application.Queries.GetFriendRequests;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Mapping;

public class FriendRequestProfile : Profile
{
    public FriendRequestProfile()
    {
        CreateMap<Guid, GetFriendRequestsQuery>()
            .ForMember(d => d.UserId, s => s.MapFrom(f => f));
        CreateMap<SendRequestToFriend, SendFriendRequestCommand>()
            .ForMember(d => d.SenderId, s => s.MapFrom(f => f.SenderId));
        CreateMap<AcceptFriend, AcceptFriendRequestCommand>()
            .ForMember(d => d.SenderId, s => s.MapFrom(f => f.SenderId));
        CreateMap<RejectFriend, RejectFriendCommand>()
            .ForMember(d => d.SenderId, s => s.MapFrom(f => f.SenderId));
    }
}