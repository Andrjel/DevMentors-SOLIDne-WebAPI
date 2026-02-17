using MySpot.App.Abstractions.Commands;
using MySpot.App.Exceptions;
using MySpot.Core.Abstractions;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.Services;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Commands;

public sealed record ReserveParkingSpotForVehicle(
    Guid ReservationId,
    Guid ParkingSpotId,
    string EmployeeName,
    string LicensePlate,
    DateTime Date,
    int Capacity
) : ICommand;

internal sealed class ReserveParkingSpotForVehicleHandler(
    IClock clock,
    IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
    IParkingReservationService parkingReservationService
) : ICommandHandler<ReserveParkingSpotForVehicle>
{
    public async Task HandleAsync(ReserveParkingSpotForVehicle command)
    {
        var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
        var week = new Week(clock.Current);
        var weeklyParkingSpots = (await weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
        if (parkingSpotToReserve is null)
            throw new WeeklyParkingSpotNotFoundException(parkingSpotId);

        var newReservation = new VehicleReservation(
            id: command.ReservationId,
            parkingSpotId: command.ParkingSpotId,
            licensePlate: command.LicensePlate,
            employeeName: command.EmployeeName,
            date: new Date(command.Date),
            capacity: command.Capacity
        );

        parkingReservationService.ReserveSpotForVehicle(
            weeklyParkingSpots,
            JobTitle.Employee,
            parkingSpotToReserve,
            newReservation
        );
        await weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);
    }
}
