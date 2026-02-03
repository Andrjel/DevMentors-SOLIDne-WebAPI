namespace MySpot.Api.Exceptions;

public class InvalidParkingSpotNameException : CustomException
{
    public InvalidParkingSpotNameException()
        : base("Invalid parking spot name.") { }
}
