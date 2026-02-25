namespace MySpot.Core.Exceptions;

public class InvalidUsernameException : CustomException
{
    public string Username { get; }

    public InvalidUsernameException(string username)
        : base($"Invalid username: {username}.") => Username = username;
}
