using Microsoft.EntityFrameworkCore;
using MySpot.Api.Endpoints;
using MySpot.App;
using MySpot.App.Services;
using MySpot.Core;
using MySpot.Core.ValueObjects;
using MySpot.Infrastructure;
using MySpot.Infrastructure.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
// app.UseHttpsRedirection();
app.MapWeatherForecastApi();
app.MapReservationsV1();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MySpotDbContext>();
    dbContext.Database.Migrate();

    var clock = scope.ServiceProvider.GetRequiredService<IClock>();
    var weeklyParkingSpots = await dbContext.WeeklyParkingSpots.ToListAsync();
    if (!weeklyParkingSpots.Any())
    {
        weeklyParkingSpots =
        [
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"), new Week(clock.Current), "P1"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"), new Week(clock.Current), "P2"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000003"), new Week(clock.Current), "P3"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000004"), new Week(clock.Current), "P4"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000005"), new Week(clock.Current), "P5"),
        ];
        dbContext.WeeklyParkingSpots.AddRange(weeklyParkingSpots);
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
