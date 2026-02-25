namespace MySpot.Core.ValueObjects;

public record UserId(Guid Value)
{
    public static implicit operator Guid(UserId id) => id.Value;

    public static implicit operator UserId(Guid id) => new(id);

    public static bool operator ==(UserId userId, Guid id) => userId.Value == id;

    public static bool operator !=(UserId userId, Guid id) => !(userId == id);
}
