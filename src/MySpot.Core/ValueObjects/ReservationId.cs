namespace MySpot.Core.ValueObjects;

public record ReservationId(Guid Value)
{
    public static implicit operator Guid(ReservationId id) => id.Value;

    public static implicit operator ReservationId(Guid id) => new(id);
}
