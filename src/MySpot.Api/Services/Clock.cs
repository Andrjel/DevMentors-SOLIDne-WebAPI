namespace MySpot.Api.Services;

public class Clock : IClock
{
    public DateTimeOffset Current => DateTimeOffset.UtcNow;
}
