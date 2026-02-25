namespace MySpot.Core.ValueObjects;

public sealed record Role
{
    public string Value { get; set; }

    public const string Employee = nameof(Employee);
    public const string Manager = nameof(Manager);
    public const string Boss = nameof(Boss);

    private Role(string value) => Value = value;

    public static implicit operator string(Role jobTitle) => jobTitle.Value;

    public static implicit operator Role(string value) => new(value);
}
