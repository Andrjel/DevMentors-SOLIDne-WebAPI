using MySpot.Core.Exceptions;

namespace MySpot.App.Exceptions;

public class InvalidCredentialsException : CustomException
{
    public InvalidCredentialsException()
        : base("Invalid credentials.") { }
}
