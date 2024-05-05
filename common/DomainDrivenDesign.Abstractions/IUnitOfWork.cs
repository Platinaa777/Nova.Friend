namespace DomainDrivenDesign.Abstractions;

public interface IUnitOfWork : IDisposable
{
    ValueTask StartTransaction(CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
}