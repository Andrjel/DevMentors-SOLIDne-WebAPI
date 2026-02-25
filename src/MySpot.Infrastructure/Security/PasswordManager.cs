using Microsoft.AspNetCore.Identity;
using MySpot.App.Security;
using MySpot.Core.Entities;

namespace MySpot.Infrastructure.Security;

public class PasswordManager : IPasswordManager
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordManager(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Secure(string password) => _passwordHasher.HashPassword(null!, password);

    public bool Validate(string password, string securedPassword) =>
        _passwordHasher.VerifyHashedPassword(null!, securedPassword, password)
        is PasswordVerificationResult.Success;
}
