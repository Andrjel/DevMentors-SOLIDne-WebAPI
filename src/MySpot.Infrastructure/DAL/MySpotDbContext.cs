using Microsoft.EntityFrameworkCore;
using MySpot.Core.Entities;

namespace MySpot.Infrastructure.DAL;

internal sealed class MySpotDbContext(DbContextOptions<MySpotDbContext> dbContextOptions)
    : DbContext(dbContextOptions)
{
    public DbSet<WeeklyParkingSpot> WeeklyParkingSpots { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
