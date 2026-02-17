using Microsoft.EntityFrameworkCore;
using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;
using MySpot.App.Queries;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Queries;

internal sealed class GetWeeklyParkingSpotsHandler(MySpotDbContext dbContext)
    : IQueryHandler<GetWeeklyParkingSpots, IEnumerable<WeeklyParkingSpotDto>>
{
    public async Task<IEnumerable<WeeklyParkingSpotDto>> HandleAsync(GetWeeklyParkingSpots query)
    {
        var week = query.Date.HasValue ? new Week(query.Date.Value) : null;
        var weeklyParkingSpots = await dbContext
            .WeeklyParkingSpots.Where(x => week == null || x.Week == week)
            .Include(x => x.Reservations)
            .AsNoTracking()
            .ToListAsync();

        return weeklyParkingSpots.Select(x => x.AsDto());
    }
}
