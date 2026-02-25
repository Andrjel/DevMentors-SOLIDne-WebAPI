using MySpot.Core.Exceptions;

namespace MySpot.Core.ValueObjects;

public sealed record Username
{
    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 30 or < 3)
            throw new InvalidUsernameException(value);
        this.Value = value;
    }

    public string Value { get; }

    public static implicit operator string(Username name) => name.Value;

    public static implicit operator Username(string name) => new(name);
}
