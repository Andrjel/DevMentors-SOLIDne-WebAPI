using MySpot.App.DTO;

namespace MySpot.App.Security;

public interface IAuthenticator
{
    JwtDto CreateToken(Guid userId, string role);
}