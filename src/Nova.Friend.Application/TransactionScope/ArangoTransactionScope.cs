using Core.Arango;
using Nova.Friend.Application.Constants;

namespace Nova.Friend.Application.TransactionScope;

public class ArangoTransactionScope : ITransactionScope
{
    public ArangoHandle TransactionId { get; set; } = new(DatabaseOptions.DatabaseName);
    public List<string> Read { get; set; } = new();
    public List<string> Write { get; set; } = new();
    public ITransactionScope AddReadScope(string collectionName)
    {
        Read.Add(collectionName);
        return this;
    }

    public ITransactionScope AddWriteScope(string collectionName)
    {
        Write.Add(collectionName);
        return this;
    }
}