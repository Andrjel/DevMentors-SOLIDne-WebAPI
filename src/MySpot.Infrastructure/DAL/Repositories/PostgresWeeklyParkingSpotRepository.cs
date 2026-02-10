using Microsoft.EntityFrameworkCore;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Repositories;

internal sealed class PostgresWeeklyParkingSpotRepository : IWeeklyParkingSpotRepository
{
    private readonly MySpotDbContext _context;

    public PostgresWeeklyParkingSpotRepository(MySpotDbContext context)
    {
        _context = context;
    }

    public WeeklyParkingSpot Get(ParkingSpotId id) =>
        _context.WeeklyParkingSpots.Include(x => x.Reservations).SingleOrDefault(x => x.Id == id);

    public IEnumerable<WeeklyParkingSpot> GetAll() =>
        _context.WeeklyParkingSpots.Include(x => x.Reservations).ToList();

    public void Add(WeeklyParkingSpot weeklyParkingSpot)
    {
        _context.WeeklyParkingSpots.Add(weeklyParkingSpot);
        _context.SaveChanges();
    }

    public void Update(WeeklyParkingSpot weeklyParkingSpot)
    {
        _context.WeeklyParkingSpots.Update(weeklyParkingSpot);
        _context.SaveChanges();
    }

    public void Delete(WeeklyParkingSpot weeklyParkingSpot)
    {
        _context.WeeklyParkingSpots.Remove(weeklyParkingSpot);
        _context.SaveChanges();
    }
}
