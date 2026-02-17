using MySpot.Core.ValueObjects;

namespace MySpot.Core.Exceptions;

public sealed class CannotReserveParkingSpotException : CustomException
{
    private readonly ParkingSpotId _parkingSpotId;

    public CannotReserveParkingSpotException(ParkingSpotId parkingSpotId)
        : base($"Cannot reserve parking spot with Id: {parkingSpotId}")
    {
        _parkingSpotId = parkingSpotId;
    }
}
