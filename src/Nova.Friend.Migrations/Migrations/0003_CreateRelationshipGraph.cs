using Core.Arango;
using Core.Arango.Migration;
using Core.Arango.Protocol;

namespace Nova.Friend.Migrations.Migrations;

public class CreateRelationshipGraph : IArangoMigration
{
    public long Id { get; } = 3;
    public string Name { get; } = "CreateRelationshipGraphMigration";
    
    public async Task Up(IArangoMigrator migrator, ArangoHandle handle)
    {
        Console.WriteLine("Applied migration:" + Id);
        await migrator.ApplyStructureAsync(Database.DatabaseName, new ArangoStructure()
        {
            Graphs = new List<ArangoGraph>()
            {
                new ArangoGraph()
                {
                    Name = "relations",
                    EdgeDefinitions = new List<ArangoEdgeDefinition>
                    {
                        new ArangoEdgeDefinition
                        {
                            Collection = "Friends",
                            From = new List<string> { "UserSnapshot" },
                            To = new List<string> { "UserSnapshot" }
                        }
                    }
                },
            }
        });
    }

    public async Task Down(IArangoMigrator migrator, ArangoHandle handle)
    {
        await migrator.Context.Graph.DropAsync(Database.DatabaseName, "relations");
    }
}