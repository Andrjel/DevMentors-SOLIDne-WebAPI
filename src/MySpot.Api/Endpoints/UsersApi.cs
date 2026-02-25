using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.App.Abstractions.Commands;
using MySpot.App.Abstractions.Queries;
using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.App.Queries;
using MySpot.App.Security;

namespace MySpot.Api.Endpoints;

public static class UsersApi
{
    public static WebApplication MapUsersApiV1(this WebApplication app)
    {
        var group = app.MapGroup("users");
        group.MapGet("", GetUsers).WithName("GetUsers");
        group.MapGet("/{id:guid}", GetUser).WithName("GetUser");
        group.MapGet("/me", GetMe).WithName("GetMe").RequireAuthorization();
        group.MapGet("/jwt", GetJwt).WithName("GetJwt");
        group.MapPost("/signup", PostSignUpUser).WithName("PostSignUpUser");
        group.MapPost("/signin", PostSignInUser).WithName("PostSignInUser");
        return app;
    }

    private static async Task<Ok<IEnumerable<UserDto>>> GetUsers(
        [AsParameters] GetUsers query,
        [FromServices] IQueryHandler<GetUsers, IEnumerable<UserDto>> commandHandler
    )
    {
        var result = await commandHandler.HandleAsync(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UserDto>> GetUser(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetUser, UserDto> commandHandler
    )
    {
        var result = await commandHandler.HandleAsync(new GetUser { UserId = id });
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetMe(
        [FromServices] IQueryHandler<GetUser, UserDto> commandHandler,
        HttpContext httpContext
    )
    {
        if (string.IsNullOrWhiteSpace(httpContext.User.Identity!.Name))
            return TypedResults.Unauthorized();
        var userId = Guid.Parse(httpContext.User.Identity.Name);
        var user = await commandHandler.HandleAsync(new GetUser() { UserId = userId });
        return TypedResults.Ok(user);
    }

    private static async Task<Ok<JwtDto>> GetJwt([FromServices] IAuthenticator authenticator)
    {
        var userId = Guid.NewGuid();
        var role = "user";
        var jwt = authenticator.CreateToken(userId, role);
        return TypedResults.Ok(jwt);
    }

    private static async Task<NoContent> PostSignUpUser(
        [FromBody] SignUp command,
        [FromServices] ICommandHandler<SignUp> commandHandler
    )
    {
        await commandHandler.HandleAsync(command with { UserId = Guid.NewGuid() });
        return TypedResults.NoContent();
    }

    private static async Task<Ok<JwtDto>> PostSignInUser(
        [FromBody] SignIn command,
        [FromServices] ICommandHandler<SignIn> commandHandler,
        [FromServices] ITokenStorage tokenStorage
    )
    {
        await commandHandler.HandleAsync(command);
        var jwt = tokenStorage.Get();
        return TypedResults.Ok(jwt);
    }
}
