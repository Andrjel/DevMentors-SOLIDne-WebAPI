using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.Core.Abstractions;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.Services;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Services;

internal sealed class ReservationsService : IReservationsService
{
    private readonly IClock _clock;
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;
    private readonly IParkingReservationService _parkingReservationService;

    public ReservationsService(
        IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
        IClock clock,
        IParkingReservationService parkingReservationService
    )
    {
        _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        _clock = clock;
        _parkingReservationService = parkingReservationService;
    }

    public async Task<ReservationDto?> GetAsync(Guid id)
    {
        var reservations = await GetAllWeeklyAsync();
        return reservations.SingleOrDefault(r => r.Id == id);
    }

    public async Task<IEnumerable<ReservationDto?>> GetAllWeeklyAsync()
    {
        var weeklyParkingSpots = await _weeklyParkingSpotRepository.GetAllAsync();

        return weeklyParkingSpots
            .SelectMany(x => x.Reservations)
            .Select(r => new ReservationDto()
            {
                Id = r.Id,
                EmployeeName = r is VehicleReservation vr ? vr.EmployeeName : string.Empty,
                ParkingSpotId = r.ParkingSpotId,
                Date = r.Date.Value.Date,
            });
    }

    public async Task<Guid?> ReserveForVehicleAsync(ReserveParkingSpotForVehicle command)
    {
        var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
        var week = new Week(_clock.Current);
        var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        var parkingSpotToReserve = weeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
        if (parkingSpotToReserve is null)
            return null;

        var newReservation = new VehicleReservation(
            id: command.ReservationId,
            parkingSpotId: command.ParkingSpotId,
            licensePlate: command.LicensePlate,
            employeeName: command.EmployeeName,
            date: new Date(command.Date),
            capacity: command.capacity
        );

        _parkingReservationService.ReserveSpotForVehicle(
            weeklyParkingSpots,
            JobTitle.Employee,
            parkingSpotToReserve,
            newReservation
        );
        await _weeklyParkingSpotRepository.UpdateAsync(parkingSpotToReserve);

        return command.ReservationId;
    }

    public async Task ReserveForCleaningAsync(ReserveParkingSpotForCleaning command)
    {
        var week = new Week(command.Date);
        var weeklyParkingSpots = (await _weeklyParkingSpotRepository.GetByWeekAsync(week)).ToList();
        _parkingReservationService.ReserveParkingForCleaning(weeklyParkingSpots, command.Date);

        // var tasks = weeklyParkingSpots.Select(x => _weeklyParkingSpotRepository.UpdateAsync(x));
        // await Task.WhenAll(tasks);

        foreach (var parkingSpot in weeklyParkingSpots)
        {
            await _weeklyParkingSpotRepository.UpdateAsync(parkingSpot);
        }
    }

    public async Task<bool> ChangeReservationmLicensePlateAsync(
        ChangeReservationLicensePlate command
    )
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
        if (weeklyParkingSpot is null)
            return false;

        var reservationId = new ReservationId(command.ReservationId);
        var existingReservation = weeklyParkingSpot
            .Reservations.OfType<VehicleReservation>()
            .SingleOrDefault(r => r.Id == reservationId);
        if (existingReservation is null)
            return false;

        if (existingReservation.Date.Value.Date <= _clock.Current)
            return false;

        existingReservation.ChangeLicensePlate(command.LicensePlate);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
        return true;
    }

    public async Task<bool> DeleteAsync(DeleteReservation command)
    {
        var weeklyParkingSpot = await GetWeeklyParkingSpotByReservationAsync(command.ReservationId);
        if (weeklyParkingSpot is null)
            return false;

        var reservationId = new ReservationId(command.ReservationId);
        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(r =>
            r.Id == reservationId
        );
        if (existingReservation is null)
            return false;

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);
        return true;
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
