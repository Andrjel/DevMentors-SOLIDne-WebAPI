using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Services;

internal sealed class ReservationsService : IReservationsService
{
    private readonly IClock _clock;
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotRepository;

    public ReservationsService(
        IWeeklyParkingSpotRepository weeklyParkingSpotRepository,
        IClock clock
    )
    {
        _weeklyParkingSpotRepository = weeklyParkingSpotRepository;
        _clock = clock;
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
                EmployeeName = r.EmployeeName,
                ParkingSpotId = r.ParkingSpotId,
                Date = r.Date.Value.Date,
            });
    }

    public async Task<Guid?> CreateAsync(CreateReservation command)
    {
        var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
        var weeklyParkingSpot = await _weeklyParkingSpotRepository.GetAsync(parkingSpotId);
        if (weeklyParkingSpot is null)
            return default;

        var newReservation = new Reservation(
            id: command.ReservationId,
            parkingSpotId: command.ParkingSpotId,
            licensePlate: command.LicensePlate,
            employeeName: command.EmployeeName,
            date: new Date(command.Date)
        );
        weeklyParkingSpot.AddReservation(newReservation, _clock.Current);
        await _weeklyParkingSpotRepository.UpdateAsync(weeklyParkingSpot);

        return command.ReservationId;
    }

    public async Task<bool> UpdateAsync(ChangeReservationLicensePlate command)
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
