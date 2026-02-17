using MySpot.Core.Exceptions;
using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public class WeeklyParkingSpot
{
    private readonly HashSet<Reservation> _reservations = [];
    public const int MaxCapacity = 2;

    public ParkingSpotId Id { get; private set; }
    public Week Week { get; private set; }
    public string Name { get; private set; }
    public Capacity Capacity { get; private set; }
    public IEnumerable<Reservation> Reservations => _reservations;

    // EF
    private WeeklyParkingSpot() { }

    private WeeklyParkingSpot(ParkingSpotId id, Week week, string name, Capacity capacity)
    {
        Id = id;
        Week = week;
        Name = name;
        Capacity = capacity;
    }

    public static WeeklyParkingSpot Create(ParkingSpotId id, Week week, string name) =>
        new(id, week, name, MaxCapacity);

    internal void AddReservation(Reservation reservation, Date now)
    {
        var isInvalidDate =
            reservation.Date < Week.From || reservation.Date > Week.To || reservation.Date < now;
        if (isInvalidDate)
            throw new InvalidReservationDateException(reservation.Date.Value.Date);

        var dateCapacity = _reservations
            .Where(x => x.Date == reservation.Date)
            .Sum(x => x.Capacity);

        if (dateCapacity + reservation.Capacity > MaxCapacity)
            throw new ParkingSpotCapacityExceededException(Id);
        _reservations.Add(reservation);
    }

    public void RemoveReservation(ReservationId reservationId)
    {
        _reservations.RemoveWhere(x => x.Id == reservationId);
    }

    public void RemoveReservations(IEnumerable<Reservation> reservations)
    {
        _reservations.RemoveWhere(x => reservations.Any(r => r.Id == x.Id));
    }
}
