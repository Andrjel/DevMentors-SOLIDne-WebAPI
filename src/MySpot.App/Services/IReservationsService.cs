using MySpot.App.Commands;
using MySpot.App.DTO;

namespace MySpot.App.Services;

public interface IReservationsService
{
    ReservationDto Get(Guid id);
    IEnumerable<ReservationDto?> GetAllWeekly();
    Guid? Create(CreateReservation command);
    bool Update(ChangeReservationLicensePlate command);
    bool Delete(DeleteReservation command);
}
