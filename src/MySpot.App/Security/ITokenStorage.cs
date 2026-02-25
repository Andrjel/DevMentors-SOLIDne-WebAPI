using MySpot.App.DTO;

namespace MySpot.App.Security;

public interface ITokenStorage
{
    void Set(JwtDto jwt);
    JwtDto Get();
}
