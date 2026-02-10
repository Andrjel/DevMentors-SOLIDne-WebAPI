namespace MySpot.App.Commands;

public record CreateReservation(
    Guid ReservationId,
    Guid ParkingSpotId,
    string EmployeeName,
    string LicensePlate,
    DateTime Date
);
