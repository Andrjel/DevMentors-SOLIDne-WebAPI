using MySpot.App.Commands;
using MySpot.App.Services;
using MySpot.Core.Abstractions;
using MySpot.Core.Policies;
using MySpot.Core.Repositories;
using MySpot.Core.Services;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Tests.Shared;
using Shouldly;

namespace MySpot.Tests.Services;

public class ReservationServiceTests
{
    #region Arrange

    private readonly IReservationsService _reservationsService;
    private readonly IWeeklyParkingSpotRepository _weeklyParkingSpotsRepository;
    private readonly IClock _clock;

    public ReservationServiceTests()
    {
        _clock = new TestClock();
        _weeklyParkingSpotsRepository = new InMemoryWeeklyParkingSpotRepository(clock: _clock);
        var parkingReservationService = new ParkingReservationService(
            [
                new RegularEmployeeReservationPolicy(_clock),
                new ManagerReservationPolicy(),
                new BossReservationPolicy(),
            ],
            _clock
        );
        _reservationsService = new ReservationsService(
            _weeklyParkingSpotsRepository,
            _clock,
            parkingReservationService
        );
    }

    #endregion

    [Fact]
    public async Task given_reservation_for_not_taken_date_create_reservation_should_succeed()
    {
        // ARRANGE
        var parkingSpot = (await _weeklyParkingSpotsRepository.GetAllAsync()).First();
        var command = new ReserveParkingSpotForVehicle(
            ReservationId: Guid.NewGuid(),
            ParkingSpotId: parkingSpot.Id,
            EmployeeName: "John Doe",
            LicensePlate: "XYZ123",
            Date: _clock.Current.DateTime.AddDays(1),
            1
        );

        // ACT
        var reservationId = await _reservationsService.ReserveForVehicleAsync(command);

        // ASSERT
        reservationId.ShouldNotBeNull();
        reservationId.Value.ShouldBe(command.ReservationId);
    }
}
