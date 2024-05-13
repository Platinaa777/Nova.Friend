using System.Collections.Concurrent;

namespace DomainDrivenDesign.Abstractions;

public class AggregateRoot<T> : Entity<T> where T : Id, IEquatable<T>
{
    private ConcurrentBag<IDomainEvent> _domainEvents = new();
    
    protected AggregateRoot(T id) : base(id)
    {
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public List<IDomainEvent> GetDomainEvents() =>
        _domainEvents.ToList();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void ChangeDomainEvents(ConcurrentBag<IDomainEvent> events)
    {
        _domainEvents = events;
    }
    
    protected AggregateRoot() {}
}