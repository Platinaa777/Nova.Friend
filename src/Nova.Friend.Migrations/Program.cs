// using Core.Arango;
// using Core.Arango.Migration;
// using Microsoft.Extensions.Configuration;
// using Nova.Friend.Application.Constants;

using Core.Arango;
using Core.Arango.Migration;

namespace Nova.Friend.Migrations;

public class Migration
{
    public static async Task Main(string[] args)
    {
        if (args.Contains("--migrate"))
        {
            await MigrateUp("--migrate", args[1], args.Length == 3);
        }
    }

    public static async Task MigrateUp(string flag, string connectionString, bool isInitial)
    {
        if (flag != "--migrate") 
            return;
        
        Console.WriteLine(Directory.GetCurrentDirectory());
        
        Console.WriteLine(connectionString);
        
        var arangoContext = new ArangoContext(connectionString);
        
        var migrationService = new ArangoMigrator(arangoContext)
        {
            HistoryCollection = "MigrationHistory"
        };
        
        migrationService.AddMigrations(typeof(Migration).Assembly);
        
        if (isInitial)
        {
            await migrationService.Context.Database.CreateAsync(DatabaseOptions.DatabaseName);
        }
        
        await migrationService.UpgradeAsync(DatabaseOptions.DatabaseName);
    }
}