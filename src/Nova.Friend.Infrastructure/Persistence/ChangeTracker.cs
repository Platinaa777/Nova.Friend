using System.Collections.Concurrent;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Infrastructure.Persistence.Abstractions;

namespace Nova.Friend.Infrastructure.Persistence;

public class ChangeTracker : IChangeTracker<ValueObject>
{
    public ConcurrentBag<AggregateRoot<ValueObject>> Entities { get; } = new();
    
    public void Track(AggregateRoot<ValueObject> entity)
    {
        Entities.Add(entity);
    }
}