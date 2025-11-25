using FluentAssertions;
using Playtesters.API.Extensions;

namespace Playtesters.API.Tests.Extensions;

public class TimeExtensionsTests
{
    [Test]
    public void ToHhMmSs_WhenZeroHours_ShouldReturn00h00m00s()
    {
        // Arrange
        double hours = 0.0;
        var expected = "00h:00m:00s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToHhMmSs_WhenExactHours_ShouldReturnCorrectFormat()
    {
        // Arrange
        double hours = 5.0;
        var expected = "05h:00m:00s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToHhMmSs_WhenFractionalHours_ShouldConvertMinutes()
    {
        // Arrange
        double hours = 2.75; // 2 hours 45 minutes
        var expected = "02h:45m:00s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToHhMmSs_WhenIncludesSeconds_ShouldConvertProperly()
    {
        // Arrange
        double hours = new TimeSpan(1, 15, 30).TotalHours;
        var expected = "01h:15m:30s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToHhMmSs_WhenMoreThan24Hours_ShouldAccumulateCorrectly()
    {
        // Arrange
        double hours = 27.25; // 27 hours 15 minutes
        var expected = "27h:15m:00s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToHhMmSs_WhenLongFractionalHours_ShouldRoundSecondsCorrectly()
    {
        // Arrange
        double hours = 0.0434027777777778; // ~2m 36s
        var expected = "00h:02m:36s";

        // Act
        string result = hours.ToHhMmSs();

        // Assert
        result.Should().Be(expected);
    }
}
