using Core.Arango;

namespace Nova.Friend.Application.TransactionScope;

public interface ITransactionScope
{
    public ArangoHandle TransactionId { get; set; }
    public List<string> Read { get; set; }
    public List<string> Write { get; set; }
    public ITransactionScope AddReadScope(string collectionName);
    public ITransactionScope AddWriteScope(string collectionName);
}