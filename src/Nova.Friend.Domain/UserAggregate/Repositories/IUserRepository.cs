using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserById(Id userId, CancellationToken cancellationToken = default);
    Task Add(User user, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
}