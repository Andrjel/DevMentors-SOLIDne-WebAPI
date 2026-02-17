using MySpot.Core.Exceptions;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Exceptions;

public sealed class ReservationNotFoundException : CustomException
{
    public ReservationNotFoundException(ReservationId reservationId)
        : base($"Reservation with Id: {reservationId} was not found.") { }
}
