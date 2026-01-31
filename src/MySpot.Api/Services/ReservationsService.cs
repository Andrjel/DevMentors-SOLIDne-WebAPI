using MySpot.Api.Models;

namespace MySpot.Api.Services;

public class ReservationsService
{
    public static int _id = 1;
    private static readonly List<Reservation> Reservations = new();
    private static readonly List<string> ParkingSpotNames = ["P1", "P2", "P3", "P4", "P5"];

    public Reservation Get(int id) => Reservations.SingleOrDefault(r => r.Id == id);

    public IEnumerable<Reservation> GetAll() => Reservations;

    public int? Create(Reservation reservation)
    {
        var now = DateTime.UtcNow.Date;
        var pastDays = now.DayOfWeek is DayOfWeek.Sunday ? 7 : (int)now.DayOfWeek;
        var remainingDays = 7 - pastDays;

        if (ParkingSpotNames.All(x => reservation.ParkingSpotName != x))
            return default;

        if (reservation.Date.Date < now.Date || reservation.Date.Date > now.AddDays(remainingDays))
            return default;

        reservation.Date = DateTime.UtcNow.AddDays(1).Date;
        var reservationAlreadyExists = Reservations.Any(x =>
            x.ParkingSpotName == reservation.ParkingSpotName && x.Date.Date == reservation.Date.Date
        );
        if (reservationAlreadyExists)
            return default;

        reservation.Id = _id++;
        Reservations.Add(reservation);
        return reservation.Id;
    }

    public bool Update(Reservation reservation)
    {
        var existingReservation = Reservations.SingleOrDefault(r => r.Id == reservation.Id);
        if (existingReservation is null)
            return false;

        if (existingReservation.Date <= DateTime.UtcNow)
            return false;

        existingReservation.LicensePlate = reservation.LicensePlate;
        return true;
    }

    public bool Delete(int id)
    {
        var existingReservation = Reservations.SingleOrDefault(r => r.Id == id);
        if (existingReservation is null)
            return false;

        Reservations.Remove(existingReservation);
        return true;
    }
}
