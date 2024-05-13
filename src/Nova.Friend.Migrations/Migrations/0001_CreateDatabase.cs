using Core.Arango;
using Core.Arango.Migration;
using Nova.Friend.Application.Constants;

namespace Nova.Friend.Migrations.Migrations;

public class CreateDatabase : IArangoMigration
{
    public long Id { get; } = 1;
    public string Name { get; } = "CreateDatabaseMigration";
    
    public async Task Up(IArangoMigrator migrator, ArangoHandle handle)
    { 
        Console.WriteLine("Applied migration:" + Id);
       
        await migrator.Context.Database.CreateAsync(DatabaseOptions.DatabaseName);
    }

    public async Task Down(IArangoMigrator migrator, ArangoHandle handle)
    {
        await migrator.Context.Database.DropAsync(DatabaseOptions.DatabaseName);
    }
}