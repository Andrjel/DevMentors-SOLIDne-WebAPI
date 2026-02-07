using System.Runtime.InteropServices;
using MySpot.Api.Entities;
using MySpot.Api.Exceptions;
using MySpot.Api.ValueObjects;
using Shouldly;

namespace MySpot.Api.Tests;

public class WeeklyParkingSpotTests
{
    #region Arrange

    private readonly WeeklyParkingSpot _weeklyParkingSpot;
    private readonly Date _now;

    public WeeklyParkingSpotTests()
    {
        _now = new DateTime(2025, 2, 3);
        _weeklyParkingSpot = new WeeklyParkingSpot(Guid.NewGuid(), new Week(_now), "P1");
    }

    #endregion

    [Theory]
    [InlineData("2026-02-02")]
    [InlineData("2026-06-06")]
    public void given_invalid_date_add_reservation_should_fail(string dateString)
    {
        // ARRANGE
        var invalidDate = DateTime.Parse(dateString);
        var reservation = new Reservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            new Date(invalidDate)
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
    public void given_reservation_for_already_existing_date_add_reservation_should_fail()
    {
        // ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new Reservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate
        );
        _weeklyParkingSpot.AddReservation(reservation, _now);
        var nextReservation = new Reservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate
        );

        // ACT
        var exception = Record.Exception(() =>
            _weeklyParkingSpot.AddReservation(nextReservation, new Date(_now))
        );

        // ASSERT
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ParkingSpotAlreadyReservedException>();
    }

    [Fact]
    public void given_reservation_for_not_taken_date_add_reservation_should_succeed()
    {
        // ARRANGE
        var reservationDate = _now.AddDays(1);
        var reservation = new Reservation(
            Guid.NewGuid(),
            _weeklyParkingSpot.Id,
            "John Doe",
            "XYZ123",
            reservationDate
        );

        // ACT
        _weeklyParkingSpot.AddReservation(reservation, _now);

        // ASSERT
        _weeklyParkingSpot.Reservations.ShouldHaveSingleItem();
    }
}
