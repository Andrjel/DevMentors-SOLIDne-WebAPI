using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;
using MySpot.Api.ValueObjects;

namespace MySpot.Api.Services;

public class ReservationsService
{
    private static IClock _clock = new Clock();
    private static readonly List<WeeklyParkingSpot> WeeklyParkingSpots =
    [
        new(Guid.Parse("00000000-0000-0000-0000-000000000001"), new Week(_clock.Current), "P1"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000002"), new Week(_clock.Current), "P2"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000003"), new Week(_clock.Current), "P3"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000004"), new Week(_clock.Current), "P4"),
        new(Guid.Parse("00000000-0000-0000-0000-000000000005"), new Week(_clock.Current), "P5"),
    ];

    public ReservationDto Get(Guid id) => GetAllWeekly()?.SingleOrDefault(x => x.Id == id);

    public IEnumerable<ReservationDto?> GetAllWeekly() =>
        WeeklyParkingSpots
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
        var weeklyParkingSpot = WeeklyParkingSpots.SingleOrDefault(x => x.Id == parkingSpotId);
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
        return true;
    }

    private WeeklyParkingSpot? GetWeeklyParkingSpotByReservation(ReservationId reservationId) =>
        WeeklyParkingSpots.SingleOrDefault(x => x.Reservations.Any(r => r.Id == reservationId));
}
