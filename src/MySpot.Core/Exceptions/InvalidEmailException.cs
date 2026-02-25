namespace MySpot.Core.Exceptions;

public class InvalidEmailException : CustomException
{
    public string Email { get; }

    public InvalidEmailException(string email)
        : base($"Email: {email} is invalid.") => Email = email;
}
