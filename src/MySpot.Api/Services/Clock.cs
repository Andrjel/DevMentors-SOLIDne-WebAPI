namespace MySpot.Api.Services;

public interface IClock
{
    DateTimeOffset Current { get; }
}

public class Clock : IClock
{
    public DateTimeOffset Current => DateTimeOffset.UtcNow;
}
