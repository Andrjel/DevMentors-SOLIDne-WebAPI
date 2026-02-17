using MySpot.App.Abstractions.Commands;
using MySpot.App.Exceptions;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Commands;

public sealed record DeleteReservation(Guid ReservationId) : ICommand;

internal sealed class DeleteReservationHandler(
    IWeeklyParkingSpotRepository weeklyParkingSpotRepository
) : ICommandHandler<DeleteReservation>
{
    public async Task HandleAsync(DeleteReservation command)
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
        if (weeklyParkingSpot is null)
            throw new WeeklyParkingSpotNotFoundException();

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        await weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
    }

    private async Task<WeeklyParkingSpot?> GetWeeklyParkingSpotByReservationAsync(
        ReservationId reservationId
    ) =>
        (await weeklyParkingSpotRepository.GetAllAsync()).SingleOrDefault(x =>
            x.Reservations.Any(r => r.Id == reservationId)
        );
}
