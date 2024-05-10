using AutoMapper;
using Nova.Friend.Application.Commands.DeleteFriend;
using Nova.Friend.Application.Queries.GetFriends;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Mapping;

public class FriendProfile : Profile
{
    public FriendProfile()
    {
        CreateMap<Guid, GetFriendsQuery>()
            .ForMember(d => d.UserId, s => s.MapFrom(f => f));
        CreateMap<DeleteFriend, DeleteFriendCommand>()
            .ForMember(d => d.SenderId, s => s.MapFrom(f => f.From))
            .ForMember(d => d.ReceiverId, s => s.MapFrom(f => f.To));
    }
}