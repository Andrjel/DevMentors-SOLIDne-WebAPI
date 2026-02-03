using MySpot.Api.Commands;
using MySpot.Api.DTO;
using MySpot.Api.Entities;

namespace MySpot.Api.Services;

public class ReservationsService
{
    private static IClock _clock = new Clock();
    private static readonly List<WeeklyParkingSpot> WeeklyParkingSpots =
    [
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            _clock.Current.DateTime,
            _clock.Current.DateTime.AddDays(7),
            "P1"
        ),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            _clock.Current.DateTime,
            _clock.Current.DateTime.AddDays(7),
            "P2"
        ),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000003"),
            _clock.Current.DateTime,
            _clock.Current.DateTime.AddDays(7),
            "P3"
        ),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000004"),
            _clock.Current.DateTime,
            _clock.Current.DateTime.AddDays(7),
            "P4"
        ),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000005"),
            _clock.Current.DateTime,
            _clock.Current.DateTime.AddDays(7),
            "P5"
        ),
    ];

    public ReservationDto Get(Guid id) => GetAllWeekly().SingleOrDefault(x => x.Id == id);

    public IEnumerable<ReservationDto> GetAllWeekly() =>
        WeeklyParkingSpots
            .SelectMany(x => x.Reservations)
            .Select(r => new ReservationDto()
            {
                Id = r.Id,
                EmployeeName = r.EmployeeName,
                ParkingSpotId = r.ParkingSpotId,
                Date = r.Date,
            });

    public Guid? Create(CreateReservation command)
    {
        var weeklyParkingSpot = WeeklyParkingSpots.SingleOrDefault(x =>
            x.Id == command.ParkingSpotId
        );
        if (weeklyParkingSpot is null)
            return default;

        var newReservation = new Reservation(
            id: command.ReservationId,
            parkingSpotId: command.ParkingSpotId,
            licensePlate: command.LicensePlate,
            employeeName: command.EmployeeName,
            date: command.Date
        );
        weeklyParkingSpot.AddReservation(newReservation, _clock.Current.DateTime);

        return command.ReservationId;
    }

    public bool Update(ChangeReservationLicensePlate command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
            return false;

        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(r =>
            r.Id == command.ReservationId
        );
        if (existingReservation is null)
            return false;

        if (existingReservation.Date <= _clock.Current.DateTime)
            return false;

        existingReservation.ChangeLicensePlate(command.LicensePlate);
        return true;
    }

    public bool Delete(DeleteReservation command)
    {
        var weeklyParkingSpot = GetWeeklyParkingSpotByReservation(command.ReservationId);
        if (weeklyParkingSpot is null)
            return false;

        var existingReservation = weeklyParkingSpot.Reservations.SingleOrDefault(r =>
            r.Id == command.ReservationId
        );
        if (existingReservation is null)
            return false;

        weeklyParkingSpot.RemoveReservation(command.ReservationId);
        return true;
    }

    private WeeklyParkingSpot GetWeeklyParkingSpotByReservation(Guid reservationId) =>
        WeeklyParkingSpots.SingleOrDefault(x => x.Reservations.Any(r => r.Id == reservationId));
}
