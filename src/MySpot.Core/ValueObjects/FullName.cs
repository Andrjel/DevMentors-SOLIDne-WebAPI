using MySpot.Core.Exceptions;

namespace MySpot.Core.ValueObjects;

public sealed record FullName
{
    public FullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 30 or < 3)
            throw new InvalidFullNameException(value);
        this.Value = value;
    }

    public string Value { get; }

    public static implicit operator string(FullName name) => name.Value;

    public static implicit operator FullName(string name) => new(name);
}
