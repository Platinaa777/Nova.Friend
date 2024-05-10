using System.Collections.Concurrent;
using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Infrastructure.Persistence.Abstractions;

public interface IChangeTracker<T> where T : ValueObject, IEquatable<T>
{
    ConcurrentBag<AggregateRoot<T>> Entities { get; }

    public void Track(AggregateRoot<T> entity);
}