using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public abstract class Reservation(ReservationId id, ParkingSpotId parkingSpotId, Date date)
{
    public ReservationId Id { get; private set; } = id;
    public ParkingSpotId ParkingSpotId { get; private set; } = parkingSpotId;
    public Date Date { get; private set; } = date;
}
