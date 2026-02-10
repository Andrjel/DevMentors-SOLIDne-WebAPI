namespace MySpot.Core.Exceptions;

public class InvalidParkingSpotNameException : CustomException
{
    public InvalidParkingSpotNameException()
        : base("Invalid parking spot name.") { }
}
