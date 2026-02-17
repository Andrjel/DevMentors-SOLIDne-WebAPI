using MySpot.Core.ValueObjects;

namespace MySpot.Core.Exceptions;

public sealed class NoReservationPolicyFoundException : CustomException
{
    private readonly JobTitle _jobTitle;

    public NoReservationPolicyFoundException(JobTitle jobTitle)
        : base($"No reservation policy for {jobTitle} has been found.")
    {
        _jobTitle = jobTitle;
    }
}
