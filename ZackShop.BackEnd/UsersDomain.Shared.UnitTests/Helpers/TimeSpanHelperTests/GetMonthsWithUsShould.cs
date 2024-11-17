using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using UsersDomain.Shared.Helpers;

namespace UsersDomain.Shared.UnitTests.Helpers.TimeSpanHelperTests;

public class GetMonthsWithUsShould
{
    [Fact]
    public void ThrowArgumentException_WhenDateTimeOffsetIsInTheFuture()
    {

        // Arrange
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset dateTimeNow = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new FakeTimeProvider(dateTimeNow);
        TimeSpanHelper sut = new TimeSpanHelper(timeProvider);
        // Act
        Action act = () => sut.GetMonthsWithUs(dateTimeOffset);
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The date must be in the past*");
    }

    [Fact]
    public void Return_0_WhenDateTimeOffsetIsToday()
    {

        // Arrange
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset dateTimeNow = new DateTimeOffset(2024, 1, 1, 3, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new FakeTimeProvider(dateTimeNow);
        TimeSpanHelper sut = new TimeSpanHelper(timeProvider);
        // Act
        int actual = sut.GetMonthsWithUs(dateTimeOffset);
        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void Return_0_WhenDateTimeOffsetIsNow()
    {

        // Arrange
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset dateTimeNow = new DateTimeOffset(2024, 1, 1, 1, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new FakeTimeProvider(dateTimeNow);
        TimeSpanHelper sut = new TimeSpanHelper(timeProvider);
        // Act
        int actual = sut.GetMonthsWithUs(dateTimeOffset);
        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void Return_1_WhenDateTimeOffsetIs1MonthBefore()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new DateTimeOffset(2023, 12, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset dateTimeNow = new DateTimeOffset(2024, 1, 1, 1, 0, 0, TimeSpan.Zero);
        FakeTimeProvider timeProvider = new FakeTimeProvider(dateTimeNow);
        TimeSpanHelper sut = new TimeSpanHelper(timeProvider);
        // Act
        int actual = sut.GetMonthsWithUs(dateTimeOffset);
        // Assert
        actual.Should().Be(1);
    }
}
