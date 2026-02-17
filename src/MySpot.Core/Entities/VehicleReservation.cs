using MySpot.Core.ValueObjects;

namespace MySpot.Core.Entities;

public sealed class VehicleReservation : Reservation
{
    public EmployeeName EmployeeName { get; private set; }
    public LicensePlate LicensePlate { get; private set; }

    public VehicleReservation(
        ReservationId id,
        ParkingSpotId parkingSpotId,
        EmployeeName employeeName,
        LicensePlate licensePlate,
        Date date,
        Capacity capacity
    )
        : base(id, parkingSpotId, date, capacity)
    {
        EmployeeName = employeeName;
        LicensePlate = licensePlate;
    }

    public void ChangeLicensePlate(LicensePlate licensePlate) => LicensePlate = licensePlate;
}
