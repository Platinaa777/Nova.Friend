using Core.Arango;
using Core.Arango.Migration;
using Core.Arango.Protocol;

namespace Nova.Friend.Migrations.Migrations;

public class CreateFriendRequestCollection : IArangoMigration
{
    public long Id { get; } = 4;
    public string Name { get; } = "CreateFriendRequestCollectionMigration";
    
    public async Task Up(IArangoMigrator migrator, ArangoHandle handle)
    {
        Console.WriteLine("Applied migration:" + Id);
        await migrator.ApplyStructureAsync(DatabaseOptions.DatabaseName, new ArangoStructure()
        {
            Collections = new List<ArangoCollectionIndices>()
            {
                new ()
                {
                    Collection = new ArangoCollection()
                    {
                        Name = DatabaseOptions.RequestCollection,
                        Type = ArangoCollectionType.Document
                    }
                }
            }
        });
    }

    public async Task Down(IArangoMigrator migrator, ArangoHandle handle)
    {
        await migrator.Context.Collection.DropAsync(DatabaseOptions.DatabaseName, DatabaseOptions.RequestCollection);
    }
}