using AutoMapper;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.Application.Queries.GetFriends;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AddUserRequest, CreateUserCommand>()
            .ForMember(x => x.UserId, s => s.MapFrom(f => f.UserId));
        
    }
}