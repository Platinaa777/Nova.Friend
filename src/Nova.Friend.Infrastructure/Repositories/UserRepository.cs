using Core.Arango;
using Core.Arango.Linq;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;

    public UserRepository(
        IArangoContext dbContext,
        ITransactionScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }
    
    public async Task<User?> FindUserById(UserId userId, CancellationToken cancellationToken = default)
    {
        UserSnapshot snapshot = await _dbContext.Query<UserSnapshot>(_scope.TransactionId)
                .FirstOrDefaultAsync(x => x.Id == userId.Value, cancellationToken);

        if (snapshot is null)
            return null;
        
        return snapshot.Restore();
    }

    public async Task Add(User user, CancellationToken cancellationToken = default)
    {
        var snapshot = user.Save();

        await _dbContext.Document.CreateAsync(_scope.TransactionId,
            DatabaseOptions.UserCollection, snapshot, cancellationToken: cancellationToken);
    }

    public async Task Update(User user, CancellationToken cancellationToken = default)
    {
        var snapshot = user.Save();

        await _dbContext.Document.UpdateAsync(_scope.TransactionId, DatabaseOptions.UserCollection, snapshot,
            cancellationToken: cancellationToken);
    }
}