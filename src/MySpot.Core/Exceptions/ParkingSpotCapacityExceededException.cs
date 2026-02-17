using MySpot.Core.ValueObjects;

namespace MySpot.Core.Exceptions;

public class ParkingSpotCapacityExceededException(ParkingSpotId parkingSpotId)
    : CustomException($"Parking spot with Id: {parkingSpotId} exceeds its reservation capacity.")
{
    public ParkingSpotId ParkingSpotId { get; } = parkingSpotId;
}
