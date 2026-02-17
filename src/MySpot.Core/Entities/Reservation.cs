using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public abstract class Reservation(
    ReservationId id,
    ParkingSpotId parkingSpotId,
    Date date,
    Capacity capacity
)
{
    public ReservationId Id { get; private set; } = id;
    public ParkingSpotId ParkingSpotId { get; private set; } = parkingSpotId;
    public Date Date { get; private set; } = date;
    public Capacity Capacity { get; private set; } = capacity;
}
