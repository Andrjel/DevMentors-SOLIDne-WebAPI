namespace MySpot.Core.ValueObjects;

public record ParkingSpotId(Guid Value)
{
    public static implicit operator Guid(ParkingSpotId id) => id.Value;

    public static implicit operator ParkingSpotId(Guid id) => new(id);
}
