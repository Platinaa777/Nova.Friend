namespace Nova.Friend.Application.TransactionScope;

public interface IUnitOfWork
{
    ValueTask StartTransaction(ITransactionScope scope, CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}