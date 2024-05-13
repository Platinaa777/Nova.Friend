using System.Collections.Concurrent;
using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Infrastructure.Persistence.Abstractions;

public interface IChangeTracker
{
    ConcurrentBag<AggregateRoot<Id>> Entities { get; }

    public void Track(AggregateRoot<Id> entity);
}