using Core.Arango;
using Core.Arango.Migration;
using Core.Arango.Protocol;
using Nova.Friend.Application.Constants;

namespace Nova.Friend.Migrations.Migrations;

public class CreateRelationshipGraph : IArangoMigration
{
    public long Id { get; } = 3;
    public string Name { get; } = "CreateRelationshipGraphMigration";
    
    public async Task Up(IArangoMigrator migrator, ArangoHandle handle)
    {
        Console.WriteLine("Applied migration:" + Id);
        await migrator.ApplyStructureAsync(DatabaseOptions.DatabaseName, new ArangoStructure()
        {
            Graphs = new List<ArangoGraph>()
            {
                new ArangoGraph()
                {
                    Name = DatabaseOptions.RelationGraph,
                    EdgeDefinitions = new List<ArangoEdgeDefinition>
                    {
                        new ArangoEdgeDefinition
                        {
                            Collection = DatabaseOptions.FriendEdge,
                            From = new List<string> { DatabaseOptions.UserCollection },
                            To = new List<string> { DatabaseOptions.UserCollection }
                        }
                    }
                },
            }
        });
    }

    public async Task Down(IArangoMigrator migrator, ArangoHandle handle)
    {
        await migrator.Context.Graph.DropAsync(DatabaseOptions.DatabaseName, DatabaseOptions.RelationGraph);
    }
}