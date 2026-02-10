using Microsoft.Extensions.DependencyInjection;
using MySpot.App.Services;

namespace MySpot.App;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReservationsService, ReservationsService>();
        return services;
    }
}
