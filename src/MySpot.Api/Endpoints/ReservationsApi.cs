using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.Services;

namespace MySpot.Api.Endpoints;

public static class ReservationsApi
{
    public static WebApplication MapReservationsV1(this WebApplication app)
    {
        var group = app.MapGroup("reservations");
        group.MapGet("", GetReservations).WithName("GetReservations");
        group.MapGet("/{id:guid}", GetReservation).WithName("GetReservation");
        group.MapPost("", PostReservations).WithName("PostReservations");
        group.MapPut("/{id:guid}", PutReservations).WithName("PutReservations");
        group.MapDelete("/{id:guid}", DeleteReservations).WithName("DeleteReservations");
        return app;
    }

    private static Results<Ok<IEnumerable<ReservationDto>>, BadRequest> GetReservations(
        [FromServices] ReservationsService reservationsService
    ) => TypedResults.Ok(reservationsService.GetAllWeekly());

    private static Results<Ok<ReservationDto>, NotFound> GetReservation(
        [FromRoute] Guid id,
        [FromServices] ReservationsService reservationsService
    )
    {
        var reservation = reservationsService.Get(id);
        if (reservation is null)
            return TypedResults.NotFound();
        return TypedResults.Ok(reservation);
    }

    private static Results<CreatedAtRoute<Reservation?>, BadRequest> PostReservations(
        [FromBody] CreateReservation command,
        [FromServices] ReservationsService reservationsService
    )
    {
        var id = reservationsService.Create(command with { ReservationId = Guid.NewGuid() });
        if (id is null)
            return TypedResults.BadRequest();
        return TypedResults.CreatedAtRoute<Reservation?>(null, "GetReservations", new { id });
    }

    private static Results<NoContent, NotFound> PutReservations(
        [FromRoute] Guid id,
        [FromBody] ChangeReservationLicensePlate command,
        [FromServices] ReservationsService reservationsService
    )
    {
        if (reservationsService.Update(command with { ReservationId = id }))
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }

    private static Results<NoContent, NotFound> DeleteReservations(
        [FromRoute] Guid id,
        [FromServices] ReservationsService reservationsService
    )
    {
        if (reservationsService.Delete(new DeleteReservation(id)))
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }
}
