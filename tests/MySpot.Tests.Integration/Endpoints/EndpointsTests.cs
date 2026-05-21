using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySpot.App.DTO;
using MySpot.App.Security;
using MySpot.Infrastructure.Auth;
using MySpot.Infrastructure.Services;

namespace MySpot.Tests.Integration.Endpoints;

[Collection("api")]
public abstract class EndpointsTests : IClassFixture<OptionsProvider>
{
    private readonly IAuthenticator _authenticator;
    protected HttpClient Client { get; }

    protected JwtDto Authorize(Guid userId, string role)
    {
        var jwt = _authenticator.CreateToken(userId, role);
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.AccessToken);
        return jwt;
    }

    public EndpointsTests(OptionsProvider optionsProvider)
    {
        var app = new MySpotTestApp(ConfigureServices);
        Client = app.Client;

        var authOptions = optionsProvider.Get<AuthOptions>("auth");
        _authenticator = new Authenticator(
            new OptionsWrapper<AuthOptions>(authOptions),
            new Clock()
        );
    }

    protected virtual void ConfigureServices(IServiceCollection services) { }
}
