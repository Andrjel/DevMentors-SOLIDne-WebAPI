namespace MySpot.Core.Abstractions;

public interface IClock
{
    DateTimeOffset Current { get; }
}
