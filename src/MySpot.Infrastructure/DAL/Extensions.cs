using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.App.Abstractions.Commands;
using MySpot.App.Commands;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL.Decorators;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Logging.Decorators;

namespace MySpot.Infrastructure.DAL;

internal static class Extensions
{
    private const string DatabaseSectionName = "Postgres";

    public static IServiceCollection AddPostgres(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection(DatabaseSectionName);
        services.Configure<PostgresOptions>(section);
        var options = configuration.GetOptions<PostgresOptions>(DatabaseSectionName);
        services.AddDbContext<MySpotDbContext>(x => x.UseNpgsql(options.ConnectionString));
        services.AddScoped<IWeeklyParkingSpotRepository, PostgresWeeklyParkingSpotRepository>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddScoped<IUnitOfWork, PostgresUnitOfWork>();

        services.TryDecorate(
            typeof(ICommandHandler<>),
            typeof(UnitOfWorkCommandHandlerDecorator<>)
        );

        services.AddHostedService<DatabaseInitializer>();
        return services;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName)
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);
        return options;
    }
}
