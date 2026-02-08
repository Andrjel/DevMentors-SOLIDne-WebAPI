namespace MySpot.Api.Services;

public interface IClock
{
    DateTimeOffset Current { get; }
}