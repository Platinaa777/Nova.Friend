using Nova.Friend.Application.TransactionScope;

namespace Nova.Friend.Infrastructure.Persistence;

public class ArangoTransactionScope : ITransactionScope
{
    public string Name { get; set; }
    public List<string> Read { get; set; }
    public List<string> Write { get; set; }

    public ArangoTransactionScope(string name, List<string> read, List<string> write)
    {
        Name = name;
        Read = read;
        Write = write;
    }
}