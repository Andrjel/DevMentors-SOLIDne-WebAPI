using MySpot.Core.Exceptions;

namespace MySpot.Core.ValueObjects;

public record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPasswordException(value);
        Value = value;
    }

    public static implicit operator string(Password password) => password.Value;

    public static implicit operator Password(string value) => new(value);
}
