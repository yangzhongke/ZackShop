using BackEnd.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BackEnd.Shared;
public static class MainHelper
{
    public static IConfigurationRoot BuildDefaultConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static void RunMigration<TDbContext>(IConfiguration configuration) where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        using var serviceProvider = new ServiceCollection()
            .AddDb<TDbContext>(configuration)
            .AddLogging(configure => configure.AddConsole())
            .BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<TDbContext>();
        dbContext.Database.Migrate();//This method is an extension method, defined in the the NuGet package Microsoft.EntityFrameworkCore.Relational.
    }

    public static void RunMigration<TDbContext>() where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        var configuration = BuildDefaultConfiguration();
        RunMigration<TDbContext>(configuration);
    }
}