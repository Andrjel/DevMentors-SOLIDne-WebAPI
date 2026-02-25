using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;

namespace MySpot.App.Queries;

public sealed record GetUser : IQuery<UserDto>
{
    public Guid UserId { get; init; }
}
