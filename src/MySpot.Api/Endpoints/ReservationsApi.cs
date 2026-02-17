using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.App.Services;
using MySpot.Core.Entities;

namespace MySpot.Api.Endpoints;

public static class ReservationsApi
{
    public static WebApplication MapReservationsV1(this WebApplication app)
    {
        var group = app.MapGroup("reservations");
        group.MapGet("", GetReservations).WithName("GetReservations");
        group.MapGet("/{id:guid}", GetReservation).WithName("GetReservation");
        group.MapPost("/vehicle", PostReservationForVehicle).WithName("PostReservationForVehicle");
        group
            .MapPost("/cleaning", PostReservationForCleaning)
            .WithName("PostReservationForCleaning");
        group.MapPut("/{id:guid}", PutReservations).WithName("PutReservations");
        group.MapDelete("/{id:guid}", DeleteReservations).WithName("DeleteReservations");
        return app;
    }

    private static async Task<Ok<IEnumerable<ReservationDto>>> GetReservations(
        [FromServices] IReservationsService reservationsService
    ) => TypedResults.Ok(await reservationsService.GetAllWeeklyAsync());

    private static async Task<Results<Ok<ReservationDto>, NotFound>> GetReservation(
        [FromRoute] Guid id,
        [FromServices] IReservationsService reservationsService
    )
    {
        var reservation = await reservationsService.GetAsync(id);
        if (reservation is null)
            return TypedResults.NotFound();
        return TypedResults.Ok(reservation);
    }

    private static async Task<
        Results<CreatedAtRoute<Reservation?>, BadRequest>
    > PostReservationForVehicle(
        [FromBody] ReserveParkingSpotForVehicle command,
        [FromServices] IReservationsService reservationsService
    )
    {
        var id = await reservationsService.ReserveForVehicleAsync(
            command with
            {
                ReservationId = Guid.NewGuid(),
            }
        );
        if (id is null)
            return TypedResults.BadRequest();
        return TypedResults.CreatedAtRoute<Reservation?>(null, "GetReservations", new { id });
    }

    private static async Task<Results<Ok, BadRequest>> PostReservationForCleaning(
        [FromBody] ReserveParkingSpotForCleaning command,
        [FromServices] IReservationsService reservationsService
    )
    {
        await reservationsService.ReserveForCleaningAsync(command);
        return TypedResults.Ok();
    }

    private static async Task<Results<NoContent, NotFound>> PutReservations(
        [FromRoute] Guid id,
        [FromBody] ChangeReservationLicensePlate command,
        [FromServices] IReservationsService reservationsService
    )
    {
        if (
            await reservationsService.ChangeReservationmLicensePlateAsync(
                command with
                {
                    ReservationId = id,
                }
            )
        )
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteReservations(
        [FromRoute] Guid id,
        [FromServices] IReservationsService reservationsService
    )
    {
        if (await reservationsService.DeleteAsync(new DeleteReservation(id)))
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }
}
