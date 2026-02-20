using MySpot.App.Abstractions.Commands;
using MySpot.Core.Repositories;
using MySpot.Core.Services;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Commands;

public sealed record ReserveParkingSpotForCleaning(DateTime Date) : ICommand;

internal sealed class ReserveParkingSpotForCleaningHandler(
    IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
    IParkingReservationService parkingReservationService
) : ICommandHandler<ReserveParkingSpotForCleaning>
{
    public async Task HandleAsync(ReserveParkingSpotForCleaning command)
    {
        var week = new Week(command.Date);
        var weeklyParkingSpots = (await weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        parkingReservationService.ReserveParkingForCleaning(weeklyParkingSpots, command.Date);

        var tasks = weeklyParkingSpots.Select(x => weeklyParkingSpotRepository.UpdateAsync(x));
        await Task.WhenAll(tasks);
    }
}
