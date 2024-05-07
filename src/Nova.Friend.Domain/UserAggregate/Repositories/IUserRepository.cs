using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserById(UserId userId, CancellationToken cancellationToken = default);
    Task Add(User user, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
}