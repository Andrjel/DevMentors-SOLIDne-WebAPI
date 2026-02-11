using MySpot.App.Commands;
using MySpot.App.DTO;

namespace MySpot.App.Services;

public interface IReservationsService
{
    Task<ReservationDto?> GetAsync(Guid id);
    Task<IEnumerable<ReservationDto?>> GetAllWeeklyAsync();
    Task<Guid?> CreateAsync(CreateReservation command);
    Task<bool> UpdateAsync(ChangeReservationLicensePlate command);
    Task<bool> DeleteAsync(DeleteReservation command);
}
