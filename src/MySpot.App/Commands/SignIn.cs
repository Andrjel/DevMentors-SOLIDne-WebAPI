using System.Security.Authentication;
using MySpot.App.Abstractions.Commands;
using MySpot.App.Security;
using MySpot.Core.Repositories;

namespace MySpot.App.Commands;

public record SignIn(string email, string password) : ICommand;

internal sealed class SignInHandler : ICommandHandler<SignIn>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticator _authenticator;
    private readonly IPasswordManager _passwordManager;
    private readonly ITokenStorage _tokenStorage;

    public SignInHandler(
        IUserRepository userRepository,
        IAuthenticator authenticator,
        IPasswordManager passwordManager,
        ITokenStorage tokenStorage
    )
    {
        _userRepository = userRepository;
        _authenticator = authenticator;
        _passwordManager = passwordManager;
        _tokenStorage = tokenStorage;
    }

    public async Task HandleAsync(SignIn command)
    {
        var user = await _userRepository.GetByEmailAsync(command.email);
        if (user is null)
            throw new InvalidCredentialException();

        if (!_passwordManager.Validate(command.password, user.Password))
            throw new InvalidCredentialException();

        var jwt = _authenticator.CreateToken(user.Id, user.Role);
        _tokenStorage.Set(jwt);
    }
}
