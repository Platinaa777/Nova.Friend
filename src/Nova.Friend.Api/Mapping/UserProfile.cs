using AutoMapper;
using Nova.Friend.Application.Queries.GetFriends;

namespace Nova.Friend.Api.ModelsMapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Guid, GetFriendsQuery>()
            .ForMember(d => d.UserId, s => s.MapFrom(f => f));
    }
}