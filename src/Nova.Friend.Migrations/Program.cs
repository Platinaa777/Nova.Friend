
using Core.Arango;
using Core.Arango.Migration;
using Microsoft.Extensions.Configuration;
using Nova.Friend.Application.Constants;
using Nova.Friend.Migrations;

Console.WriteLine(Directory.GetCurrentDirectory());

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var arangoConnectionString = configuration.GetConnectionString("Arango");
Console.WriteLine(arangoConnectionString);

var arangoContext = new ArangoContext(arangoConnectionString);

var migrationService = new ArangoMigrator(arangoContext)
{
    HistoryCollection = "MigrationHistory"
};

migrationService.AddMigrations(typeof(Program).Assembly);
await migrationService.UpgradeAsync(DatabaseOptions.DatabaseName);
