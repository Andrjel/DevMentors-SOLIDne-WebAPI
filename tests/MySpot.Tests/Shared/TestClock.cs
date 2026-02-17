using MySpot.App.Services;
using MySpot.Core.Abstractions;

namespace MySpot.Tests.Shared;

public class TestClock : IClock
{
    public DateTimeOffset Current => new DateTimeOffset(2026, 2, 2, 12, 0, 0, TimeSpan.Zero);
}
