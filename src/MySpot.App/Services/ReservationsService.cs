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

    public ReservationDto Get(Guid id) => GetAllWeekly()?.SingleOrDefault(x => x.Id == id);

    public IEnumerable<ReservationDto?> GetAllWeekly() =>
        _weeklyParkingSpotRepository
            .GetAll()
            .SelectMany(x => x.Reservations)
            .Select(r => new ReservationDto()
            {
                Id = r.Id,
                EmployeeName = r.EmployeeName,
                ParkingSpotId = r.ParkingSpotId,
                Date = r.Date.Value.Date,
            });

    public Guid? Create(CreateReservation command)
    {
        var parkingSpotId = new ParkingSpotId(command.ParkingSpotId);
        var weeklyParkingSpot = _weeklyParkingSpotRepository.Get(parkingSpotId);
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
        _weeklyParkingSpotRepository.Update(weeklyParkingSpot);

        return command.ReservationId;
    }

    public bool Update(ChangeReservationLicensePlate command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
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
        _weeklyParkingSpotRepository.Update(weeklyParkingSpot);
        return true;
    }

    public bool Delete(DeleteReservation command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
            return false;

        var reservationId = new ReservationId(command.ReservationId);
        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(r =>
            r.Id == reservationId
        );
        if (existingReservation is null)
            return false;

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        _weeklyParkingSpotRepository.Update(weeklyParkingSpot);
        return true;
    }

    private WeeklyParkingSpot? GetWeeklyParkingSpotByReservation(ReservationId reservationId) =>
        _weeklyParkingSpotRepository
            .GetAll()
            .SingleOrDefault(x => x.Reservations.Any(r => r.Id == reservationId));
}
