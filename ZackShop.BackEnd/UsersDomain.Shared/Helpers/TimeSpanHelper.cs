using BackEnd.Shared;

namespace UsersDomain.Shared.Helpers;
public interface ITimeSpanHelper:IService
{
    public int GetMonthsWithUs(DateTimeOffset dateTimeOffset);
}

//Point 1: make TimeSpanHelper a non-static class to make it can be mocked in unit tests rather than using a static method, so that the unit tests for one single class can be isolated from the other classes
//Point 2: using TimeProvider to get the current time to let the unit tests of the classes using TimeSpanHelper can be tested without the dependency of the system time
public class TimeSpanHelper : ITimeSpanHelper
{
    private readonly TimeProvider timeProvider;

    public TimeSpanHelper(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public int GetMonthsWithUs(DateTimeOffset dateTimeOffset)
    {
        var utcNow = timeProvider.GetUtcNow();
        if (dateTimeOffset > utcNow)
        {
            throw new ArgumentException("The date must be in the past", nameof(dateTimeOffset));
        }
        const int monthsInYear = 12;
        return utcNow.Year * monthsInYear + utcNow.Month - dateTimeOffset.Year * monthsInYear - dateTimeOffset.Month;
    }
}