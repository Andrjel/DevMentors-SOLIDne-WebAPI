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

    public Task<WeeklyParkingSpot> GetAsync(ParkingSpotId id) =>
        _context
            .WeeklyParkingSpots.Include(x => x.Reservations)
            .SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<WeeklyParkingSpot>> GetAllAsync()
    {
        var result = await _context.WeeklyParkingSpots.Include(x => x.Reservations).ToListAsync();
        return result;
    }

    public async Task AddAsync(WeeklyParkingSpot weeklyParkingSpot)
    {
        await _context.WeeklyParkingSpots.AddAsync(weeklyParkingSpot);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WeeklyParkingSpot weeklyParkingSpot)
    {
        _context.WeeklyParkingSpots.Update(weeklyParkingSpot);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WeeklyParkingSpot weeklyParkingSpot)
    {
        _context.WeeklyParkingSpots.Remove(weeklyParkingSpot);
        await _context.SaveChangesAsync();
    }
}
