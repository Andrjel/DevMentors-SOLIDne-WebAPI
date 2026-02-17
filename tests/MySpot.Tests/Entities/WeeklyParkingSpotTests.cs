using MySpot.Core.Entities;
using MySpot.Core.Exceptions;
using MySpot.Core.ValueObjects;
using Shouldly;

namespace MySpot.Tests.Entities;

public class WeeklyParkingSpotTests
{
    #region Arrange

    private readonly WeeklyParkingSpot _weeklyParkingSpot;
    private readonly Date _now;

    public WeeklyParkingSpotTests()
    {
        _now = new DateTime(2025, 2, 3);
        _weeklyParkingSpot = WeeklyParkingSpot.Create(Guid.NewGuid(), new Week(_now), "P1");
    }

    #endregion

    [Theory]
    [InlineData("2026-02-02")]
    [InlineData("2026-06-06")]
    public void given_invalid_date_add_reservation_should_fail(string dateString)
    {
        // ARRANGE
        var invalidDate = DateTime.Parse(dateString);
        var reservation = new VehicleReservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            new Date(invalidDate),
            1
        );

        // ACT
        var exception = Record.Exception(() =>
            _weeklyParkingSpot.AddReservation(reservation, new Date(_now))
        );

        // ASSERT
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidReservationDateException>();
    }

    [Fact]
    public void given_reservation_for_already_reserved_parking_spot_add_reservation_should_fail()
    {
        // ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new VehicleReservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate,
            2
        );
        _weeklyParkingSpot.AddReservation(reservation, _now);
        var nextReservation = new VehicleReservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate,
            1
        );

        // ACT
        var exception = Record.Exception(() =>
            _weeklyParkingSpot.AddReservation(nextReservation, new Date(_now))
        );

        // ASSERT
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ParkingSpotCapacityExceededException>();
    }

    [Fact]
    public void given_reservation_for_not_reserved_parking_spot_add_reservation_should_succeed()
    {
        // ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new VehicleReservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate,
            1
        );

        // ACT
        _weeklyParkingSpot.AddReservation(reservation, _now);

        // ASSERT
        _weeklyParkingSpot.Reservations.ShouldHaveSingleItem();
    }
}
