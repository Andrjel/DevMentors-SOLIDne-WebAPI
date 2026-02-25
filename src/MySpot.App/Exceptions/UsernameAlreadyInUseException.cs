using MySpot.Core.Exceptions;

namespace MySpot.App.Exceptions;

public class UsernameAlreadyInUseException : CustomException
{
    public string Username { get; }

    public UsernameAlreadyInUseException(string username)
        : base($"Username `{username}` is already in use.") => Username = username;
}
