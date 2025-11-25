using FluentValidation.TestHelper;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Tests.Validators;

public class UpdatePlaytimeValidatorTests
{
    [Test]
    public void HoursPlayed_WhenGreaterThanZero_ShouldNotHaveError()
    {
        // Arrange
        var validator = new UpdatePlaytimeValidator();
        var request = new UpdatePlaytimeRequest(HoursPlayed: 2.5);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.HoursPlayed);
    }

    [Test]
    public void HoursPlayed_WhenZero_ShouldNotHaveError()
    {
        // Arrange
        var validator = new UpdatePlaytimeValidator();
        var request = new UpdatePlaytimeRequest(HoursPlayed: 0);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.HoursPlayed);
    }

    [Test]
    public void HoursPlayed_WhenNegative_ShouldHaveError()
    {
        // Arrange
        var validator = new UpdatePlaytimeValidator();
        var request = new UpdatePlaytimeRequest(HoursPlayed: -1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.HoursPlayed);
    }
}
