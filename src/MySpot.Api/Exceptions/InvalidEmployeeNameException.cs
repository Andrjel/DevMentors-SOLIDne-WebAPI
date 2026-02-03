namespace MySpot.Api.Exceptions;

public class InvalidEmployeeNameException : CustomException
{
    public InvalidEmployeeNameException()
        : base("Invalid employee name.") { }
}
