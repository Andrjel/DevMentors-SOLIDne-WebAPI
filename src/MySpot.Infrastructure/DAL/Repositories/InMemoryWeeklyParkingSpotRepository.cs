using MySpot.App.Services;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Repositories;

internal sealed class InMemoryWeeklyParkingSpotRepository : IWeeklyParkingSpotRepository
{
    private readonly List<WeeklyParkingSpot> _weeklyParkingSpots;
    private readonly IClock _clock;

    public InMemoryWeeklyParkingSpotRepository(IClock clock)
    {
        _clock = clock;
        _weeklyParkingSpots =
        [
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"), new Week(_clock.Current), "P1"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"), new Week(_clock.Current), "P2"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000003"), new Week(_clock.Current), "P3"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000004"), new Week(_clock.Current), "P4"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000005"), new Week(_clock.Current), "P5"),
        ];
    }

    public WeeklyParkingSpot Get(ParkingSpotId id) =>
        _weeklyParkingSpots.SingleOrDefault(x => x.Id == id);

    public IEnumerable<WeeklyParkingSpot> GetAll() => _weeklyParkingSpots;

    public void Add(WeeklyParkingSpot weeklyParkingSpot) =>
        _weeklyParkingSpots.Add(weeklyParkingSpot);

    public void Update(WeeklyParkingSpot weeklyParkingSpot) { }

    public void Delete(WeeklyParkingSpot weeklyParkingSpot) =>
        _weeklyParkingSpots.Remove(weeklyParkingSpot);
}
