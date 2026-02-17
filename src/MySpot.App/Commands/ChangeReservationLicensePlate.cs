using MySpot.App.Abstractions.Commands;
using MySpot.App.Exceptions;
using MySpot.Core.Abstractions;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Commands;

public sealed record ChangeReservationLicensePlate(Guid ReservationId, string LicensePlate)
    : ICommand;

internal sealed class ChangeReservationLicensePlateHandler
    : ICommandHandler<ChangeReservationLicensePlate>
{
    private IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
    private IClock _clock;

    public ChangeReservationLicensePlateHandler(
        IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
        IClock clock
    )
    {
        _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        _clock = clock;
    }

    public async Task HandleAsync(ChangeReservationLicensePlate command)
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
        if (weeklyParkingSpot is null)
            throw new WeeklyParkingSpotNotFoundException();

        var reservationId = new ReservationId(command.ReservationId);
        var existingReservation = weeklyParkingSpot
            .Reservations.OfType<VehicleReservation>()
            .SingleOrDefault(r => r.Id == reservationId);
        if (existingReservation is null)
            throw new ReservationNotFoundException(reservationId);

        existingReservation.ChangeLicensePlate(command.LicensePlate);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
    }

    private async Task<WeeklyParkingSpot?> GetWeeklyParkingSpotByReservationAsync(
        ReservationId reservationId
    )
    {
        var weeklyParkingSpots = await _weeklyParkingSpotRepository.GetAllAsync();

        return weeklyParkingSpots.SingleOrDefault(x =>
            x.Reservations.Any(r => r.Id == reservationId)
        );
    }
}
