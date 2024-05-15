using Core.Arango;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nova.Friend.Migrations;
using Quartz;
using Quartz.Logging;
using Testcontainers.ArangoDb;

namespace Nova.Friend.IntegrationTests;

public class IntegrationTestingWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly ArangoDbContainer _dbContainer = new ArangoDbBuilder()
        .WithImage("arangodb:3.12.0.2")
        .WithPassword("test-pass")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(l => l.AddProvider(NullLoggerProvider.Instance));
        
        string connectionString =
            $"Server={_dbContainer.GetTransportAddress()};Realm=nova;User=root;Password=test-pass;";
        
        builder.ConfigureTestServices(services =>
        {
            services.AddArango((_, cfg) =>
            {
                cfg.ConnectionString = connectionString;
            });
            
            LogProvider.SetCurrentLogProvider(default);
        });
    }

    public virtual async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        string connectionString = $"Server={_dbContainer.GetTransportAddress()};Realm=nova;User=root;Password=test-pass;";
        await Migration.MigrateUp("--migrate", connectionString, true);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}