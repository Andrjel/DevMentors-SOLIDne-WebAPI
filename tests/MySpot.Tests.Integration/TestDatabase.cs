using Microsoft.EntityFrameworkCore;
using MySpot.Infrastructure.DAL;

namespace MySpot.Tests.Integration;

internal class TestDatabase : IAsyncLifetime
{
    public MySpotDbContext DbContext { get; }

    public TestDatabase()
    {
        var options = new OptionsProvider().Get<PostgresOptions>("Postgres");
        DbContext = new MySpotDbContext(
            new DbContextOptionsBuilder<MySpotDbContext>()
                .UseNpgsql(options.ConnectionString)
                .Options
        );
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }
}
