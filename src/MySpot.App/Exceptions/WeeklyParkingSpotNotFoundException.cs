using MySpot.Core.Exceptions;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Exceptions;

public class WeeklyParkingSpotNotFoundException : CustomException
{
    private readonly ParkingSpotId? _parkingSpotId;

    public WeeklyParkingSpotNotFoundException(ParkingSpotId parkingSpotId)
        : base($"Weekly parking spot with id: {parkingSpotId} was not found.")
    {
        _parkingSpotId = parkingSpotId;
    }

    public WeeklyParkingSpotNotFoundException()
        : base("Weekly parking spot not found,") { }
}
