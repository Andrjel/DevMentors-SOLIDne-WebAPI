using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySpot.Infrastructure;

namespace MySpot.Api.Endpoints;

public static class HomeApi
{
    public static WebApplication MapHomeV1(this WebApplication app)
    {
        var group = app.MapGroup("");
        group.MapGet("", Get).WithName("Get");
        return app;
    }

    private static async Task<Ok<string>> Get([FromServices] IOptions<AppOptions> options)
    {
        await Task.Yield();
        return TypedResults.Ok($"Welcome to {options.Value.Name}!");
    }
}
