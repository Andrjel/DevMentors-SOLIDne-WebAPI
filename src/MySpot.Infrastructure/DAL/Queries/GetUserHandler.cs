using Microsoft.EntityFrameworkCore;
using MySpot.App.Abstractions.Queries;
using MySpot.App.DTO;
using MySpot.App.Queries;

namespace MySpot.Infrastructure.DAL.Queries;

internal sealed class GetUserHandler: IQueryHandler<GetUser, UserDto>
{
    private readonly MySpotDbContext _dbContext;

    public GetUserHandler(MySpotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto> HandleAsync(GetUser query)
        => (await _dbContext.Users.Select(x => new UserDto()
        {
            Id = x.Id,
            FullName = x.FullName,
            UserName = x.Username
        }).SingleOrDefaultAsync(x => x.Id == query.UserId))!;
}