namespace MySpot.App.Commands;

public record ReserveParkingSpotForVehicle(
    Guid ReservationId,
    Guid ParkingSpotId,
    string EmployeeName,
    string LicensePlate,
    DateTime Date
);
