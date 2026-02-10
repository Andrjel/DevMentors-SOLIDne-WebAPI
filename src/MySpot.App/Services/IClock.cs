namespace MySpot.App.Services;

public interface IClock
{
    DateTimeOffset Current { get; }
}
