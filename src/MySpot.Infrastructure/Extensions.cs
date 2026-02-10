using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.App.Services;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Services;

namespace MySpot.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("APP");
        services.Configure<AppOptions>(section);
        services.AddPostgres(configuration);
        services.AddSingleton<IClock, Clock>();
        return services;
    }
}
