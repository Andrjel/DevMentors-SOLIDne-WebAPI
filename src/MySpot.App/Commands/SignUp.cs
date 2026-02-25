using MySpot.App.Abstractions.Commands;
using MySpot.App.Exceptions;
using MySpot.App.Security;
using MySpot.Core.Abstractions;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.App.Commands;

public record SignUp(
    Guid UserId,
    string Email,
    string Username,
    string Password,
    string FullName,
    string Role
) : ICommand { }

internal sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly IUserRepository _userRepository;

    public SignUpHandler(
        IPasswordManager passwordManager,
        IClock clock,
        IUserRepository userRepository
    )
    {
        _passwordManager = passwordManager;
        _clock = clock;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(SignUp command)
    {
        var userId = new UserId(command.UserId);
        var email = new Email(command.Email);
        var username = new Username(command.Username);
        var password = new Password(command.Password);
        var fullName = new FullName(command.FullName);
        var role = string.IsNullOrWhiteSpace(command.Role) ? Role.Employee : Role.Manager;

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new EmailAlreadyInUseException(email);

        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new UsernameAlreadyInUseException(username);

        var securedPassword = _passwordManager.Secure(password);
        var user = new User(
            userId,
            email,
            username,
            securedPassword,
            fullName,
            role,
            _clock.Current.DateTime
        );
        await _userRepository.AddAsync(user);
    }
}
