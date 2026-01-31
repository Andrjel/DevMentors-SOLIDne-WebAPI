using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySpot.Api.Models;
using MySpot.Api.Services;

namespace MySpot.Api.Endpoints;

public static class ReservationsApi
{
    public static WebApplication MapReservationsV1(this WebApplication app)
    {
        var group = app.MapGroup("reservations");
        group.MapGet("", GetReservations).WithName("GetReservations");
        group.MapGet("/{id:int}", GetReservation).WithName("GetReservation");
        group.MapPost("", PostReservations).WithName("PostReservations");
        group.MapPut("/{id:int}", PutReservations).WithName("PutReservations");
        group.MapDelete("/{id:int}", DeleteReservations).WithName("DeleteReservations");
        return app;
    }

    private static Results<Ok<IEnumerable<Reservation>>, BadRequest> GetReservations(
        [FromServices] ReservationsService reservationsService
    ) => TypedResults.Ok(reservationsService.GetAll());

    private static Results<Ok<Reservation>, NotFound> GetReservation(
        [FromRoute] int id,
        [FromServices] ReservationsService reservationsService
    )
    {
        var reservation = reservationsService.Get(id);
        if (reservation is null)
            return TypedResults.NotFound();
        return TypedResults.Ok(reservation);
    }

    private static Results<CreatedAtRoute<Reservation?>, BadRequest> PostReservations(
        [FromBody] Reservation reservation,
        [FromServices] ReservationsService reservationsService
    )
    {
        var id = reservationsService.Create(reservation);
        if (id is null)
            return TypedResults.BadRequest();
        return TypedResults.CreatedAtRoute<Reservation?>(null, "GetReservations", new { id });
    }

    private static Results<NoContent, NotFound> PutReservations(
        [FromRoute] int id,
        [FromBody] Reservation reservation,
        [FromServices] ReservationsService reservationsService
    )
    {
        reservation.Id = id;
        if (reservationsService.Update(reservation))
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }

    private static Results<NoContent, NotFound> DeleteReservations(
        [FromRoute] int id,
        [FromServices] ReservationsService reservationsService
    )
    {
        if (reservationsService.Delete(id))
            return TypedResults.NoContent();
        return TypedResults.NotFound();
    }
}
