using MySpot.App.Services;

namespace MySpot.Infrastructure.Services;

internal sealed class Clock : IClock
{
    public DateTimeOffset Current => DateTimeOffset.UtcNow;
}
