using FluentValidation.TestHelper;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Tests.Validators;

public class GetTestersValidatorTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("CreatedAtDesc")]
    [TestCase("createdatdesc")]
    [TestCase("TotalHoursPlayedAsc")]
    [TestCase("totalhoursplayedasc")]
    public void OrderBy_WhenValidValue_ShouldNotHaveValidationErrors(string orderBy)
    {
        // Arrange
        var validator = new GetTestersValidator();
        var request = new GetTestersRequest(OrderBy: orderBy);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.OrderBy);
    }

    [TestCase("InvalidValue")]
    [TestCase("1234")]
    [TestCase("///")]
    [TestCase("CreatedAtDescending")]
    public void OrderBy_WhenInvalidValue_ShouldHaveValidationError(string orderBy)
    {
        // Arrange
        var validator = new GetTestersValidator();
        var request = new GetTestersRequest(OrderBy: orderBy);
        var allowed = string.Join(", ", Enum.GetNames<GetTestersOrderBy>());
        var expectedMessage = $"Invalid orderBy '{orderBy}'. Allowed values: {allowed}";

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.OrderBy)
              .WithErrorMessage(expectedMessage);
    }
}
