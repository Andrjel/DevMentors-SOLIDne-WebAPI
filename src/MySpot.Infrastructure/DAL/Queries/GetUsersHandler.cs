using Microsoft.EntityFrameworkCore;
using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;
using MySpot.App.Queries;

namespace MySpot.Infrastructure.DAL.Queries;

internal sealed class GetUsersHandler: IQueryHandler<GetUsers, IEnumerable<UserDto>>
{
    private readonly MySpotDbContext _dbContext;

    public GetUsersHandler(MySpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsers query)
        => await _dbContext.Users.Select(x => new UserDto()
        {
            Id = x.Id,
            FullName = x.FullName,
            UserName = x.Username
        }).ToListAsync();
}