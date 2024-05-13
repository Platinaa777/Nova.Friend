using System.Transactions;
using Core.Arango;
using Core.Arango.Protocol;
using DomainDrivenDesign.Abstractions;
using Newtonsoft.Json;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Infrastructure.OutboxPattern;
using Nova.Friend.Infrastructure.Persistence.Abstractions;
using ArangoTransactionScope = Core.Arango.Protocol.ArangoTransactionScope;

namespace Nova.Friend.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IChangeTracker _tracker;
    private readonly IArangoContext _arango;
    private readonly ITransactionScope _scope;
    private ArangoHandle? _transaction;

    public UnitOfWork(
        IChangeTracker tracker,
        IArangoContext arango,
        ITransactionScope scope)
    {
        _tracker = tracker;
        _arango = arango;
        _scope = scope;
    }

    public async ValueTask StartTransaction(CancellationToken token = default)
    {
        if (_transaction is not null) return;
        
        var transaction = await _arango.Transaction.BeginAsync(DatabaseOptions.DatabaseName,
            new ArangoTransaction
            { Collections = new ArangoTransactionScope { Read = _scope.Read, Write = _scope.Write } }, token);
        _transaction = transaction;
        
        _scope.TransactionId = transaction;
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        if (_transaction is null)
            throw new TransactionException("Transaction was not started");

        var domainEvents = new List<OutboxMessage>(
            _tracker.Entities.SelectMany(events =>
            {
                var collection = events.GetDomainEvents();
                events.ClearDomainEvents();
                return collection;
            }).Select(domainEvent =>
            {
                string identity = Guid.NewGuid().ToString();
                return new OutboxMessage()
                {
                    Key = identity,
                    Id = identity,
                    Type = domainEvent.GetType().Name,
                    StartedAtUtc = DateTime.UtcNow,
                    Content = JsonConvert.SerializeObject(domainEvent,
                        new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All })
                };
            })).ToList();

        if (domainEvents.Any())
        {
            await _arango.Document.CreateManyAsync(_transaction, DatabaseOptions.OutboxMessage,
                domainEvents, cancellationToken: token);    
        }

        await _arango.Transaction.CommitAsync(_transaction, token);
    }
    
    public void Dispose()
    {
    }
}