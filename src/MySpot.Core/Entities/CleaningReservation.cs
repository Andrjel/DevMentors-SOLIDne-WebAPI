using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public sealed class CleaningReservation(ReservationId id, ParkingSpotId parkingSpotId, Date date)
    : Reservation(id, parkingSpotId, date, 2) { }
