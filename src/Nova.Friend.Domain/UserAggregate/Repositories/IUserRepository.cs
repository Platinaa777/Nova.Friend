using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserById(UserId userId);
    Task Add(User user);
    Task Update(User user);
}