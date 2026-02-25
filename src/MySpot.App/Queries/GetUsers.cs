using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;

namespace MySpot.App.Queries;

public sealed record GetUsers : IQuery<IEnumerable<UserDto>>
{
}