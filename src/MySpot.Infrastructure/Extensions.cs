using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.App.Abstractions.Queries;
using MySpot.Core.Abstractions;
using MySpot.Infrastructure.Auth;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Security;
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
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<ExceptionMiddleware>();
        services.AddSecurity();
        services.AddAuth(configuration);
        services.AddHttpContextAccessor();

        services.AddPostgres(configuration);
        services.AddCustomLogging();

        var infrastructureAssembly = typeof(AppOptions).Assembly;
        services.Scan(s =>
            s.FromAssemblies(infrastructureAssembly)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new() { Title = "MySpot API", Version = "v1" });
        });
        services.AddEndpointsApiExplorer();
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseReDoc(options =>
        {
            options.DocumentTitle = "MySpot API Documentation";
            options.RoutePrefix = "docs";
            options.SpecUrl("/swagger/v1/swagger.json");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
