using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.App.Abstractions.Commands;
using MySpot.App.Abstractions.Queries;
using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.App.Queries;

namespace MySpot.Api.Endpoints;

public static class ParkingSpotsApi
{
    public static WebApplication MapParkingSpotsV1(this WebApplication app)
    {
        var group = app.MapGroup("parking-spots");
        group.MapGet("", GetWeeklyParkingSpots).WithName("GetWeeklyParkingSpots");
        group
            .MapPost("{parkingSpotId:guid}/reservations/vehicle", PostReservationForVehicle)
            .WithName("PostReservationForVehicle");
        group
            .MapPost("/reservations/cleaning", PostReservationForCleaning)
            .WithName("PostReservationForCleaning");
        group
            .MapPut("/reservations/{reservationId:guid}", PutReservations)
            .WithName("PutReservation");
        group
            .MapDelete("/reservations/{reservationId:guid}", DeleteReservations)
            .WithName("DeleteReservation");
        return app;
    }

    private static async Task<Ok<IEnumerable<WeeklyParkingSpotDto>>> GetWeeklyParkingSpots(
        [AsParameters] GetWeeklyParkingSpots query,
        [FromServices]
            IQueryHandler<GetWeeklyParkingSpots, IEnumerable<WeeklyParkingSpotDto>> queryHandler
    ) => TypedResults.Ok(await queryHandler.HandleAsync(query));

    private static async Task<Results<NoContent, BadRequest>> PostReservationForVehicle(
        [FromRoute] Guid parkingSpotId,
        [FromBody] ReserveParkingSpotForVehicle command,
        [FromServices] ICommandHandler<ReserveParkingSpotForVehicle> commandHandler
    )
    {
        await commandHandler.HandleAsync(
            command with
            {
                ReservationId = Guid.NewGuid(),
                ParkingSpotId = parkingSpotId,
            }
        );
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok, BadRequest>> PostReservationForCleaning(
        [FromBody] ReserveParkingSpotForCleaning command,
        [FromServices] ICommandHandler<ReserveParkingSpotForCleaning> commandHandler
    )
    {
        await commandHandler.HandleAsync(command);
        return TypedResults.Ok();
    }

    private static async Task<Results<NoContent, NotFound>> PutReservations(
        [FromRoute] Guid reservationId,
        [FromBody] ChangeReservationLicensePlate command,
        [FromServices] ICommandHandler<ChangeReservationLicensePlate> commandHandler
    )
    {
        await commandHandler.HandleAsync(command with { ReservationId = reservationId });
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteReservations(
        [AsParameters] DeleteReservation command,
        [FromServices] ICommandHandler<DeleteReservation> commandHandler
    )
    {
        await commandHandler.HandleAsync(command);
        return TypedResults.NoContent();
    }
}
