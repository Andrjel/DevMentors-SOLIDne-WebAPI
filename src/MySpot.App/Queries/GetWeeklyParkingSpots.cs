using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;

namespace MySpot.App.Queries;

public record GetWeeklyParkingSpots() : IQuery<IEnumerable<WeeklyParkingSpotDto>>
{
    public DateTime? Date { get; init; }
}
