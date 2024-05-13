using System.Collections.Concurrent;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Infrastructure.Persistence.Abstractions;

namespace Nova.Friend.Infrastructure.Persistence;

public class ChangeTracker : IChangeTracker
{
    public ConcurrentBag<AggregateRoot<Id>> Entities { get; } = new();
    public void Track(AggregateRoot<Id> entity)
    {
        Entities.Add(entity);
    }
}