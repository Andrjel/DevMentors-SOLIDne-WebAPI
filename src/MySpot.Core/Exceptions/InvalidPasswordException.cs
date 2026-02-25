namespace MySpot.Core.Exceptions;

public sealed class InvalidPasswordException : CustomException
{
    public string Password { get; }

    public InvalidPasswordException(string password)
        : base($"Invalid password: {password}.") => Password = password;
}
