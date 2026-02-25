using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.App.Abstractions.Commands;
using MySpot.App.Abstractions.Queries;
using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.App.Queries;

namespace MySpot.Api.Endpoints;

public static class UsersApi
{
    public static WebApplication MapUsersApiV1(this WebApplication app)
    {
        var group = app.MapGroup("users");
        group.MapGet("", GetUsers).WithName("GetUsers");
        group.MapGet("/{id:guid}", GetUser).WithName("GetUser");
        group.MapPost("/signup", PostSignUpUser).WithName("PostSignUpUser");
        return app;
    }

    private static async Task<Ok<IEnumerable<UserDto>>> GetUsers([AsParameters] GetUsers query,
        [FromServices] IQueryHandler<GetUsers, IEnumerable<UserDto>> commandHandler)
    {
        var result = await commandHandler.HandleAsync(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UserDto>> GetUser([FromRoute] Guid id,
        [FromServices] IQueryHandler<GetUser, UserDto> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new GetUser { UserId = id });
        return TypedResults.Ok(result);
    }

    
    private static async Task<NoContent> PostSignUpUser(
        [FromBody] SignUp command,
        [FromServices] ICommandHandler<SignUp> commandHandler
    )
    {
        await commandHandler.HandleAsync(command with { UserId = Guid.NewGuid() });
        return TypedResults.NoContent();
    }
}
