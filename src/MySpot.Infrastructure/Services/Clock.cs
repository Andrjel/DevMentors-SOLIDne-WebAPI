using MySpot.Core.Abstractions;

namespace MySpot.Infrastructure.Services;

internal sealed class Clock : IClock
{
    public DateTimeOffset Current => DateTimeOffset.UtcNow;
}
