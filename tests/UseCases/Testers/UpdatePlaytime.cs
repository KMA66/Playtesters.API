using FluentAssertions;
using Playtesters.API.Entities;
using Playtesters.API.Tests.Common;
using Playtesters.API.UseCases.Testers;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Tests.UseCases.Testers;

public class UpdatePlaytimeApiTests : TestBase
{
    [Test]
    public async Task Patch_WhenPlaytimeUpdated_ShouldIncreaseTotalHoursPlayed()
    {
        // Arrange
        var patchClient = ApplicationFactory.CreateClient();
        var postClient = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Alice");
        var createResponse = await postClient.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var tester = createdBody.Data;

        var updateRequest = new UpdatePlaytimeRequest(HoursPlayed: 2.5);
        var requestUri = $"/api/testers/{tester.AccessKey}/playtime";
        double expectedHoursPlayed = 2.5f;

        // Act
        var response = await patchClient.PatchAsJsonAsync(requestUri, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTester = await FirstOrDefaultAsync<Tester>(t => t.AccessKey == tester.AccessKey);
        updatedTester.TotalHoursPlayed.Should().Be(expectedHoursPlayed);
    }

    [Test]
    public async Task Patch_WhenMultipleUpdates_ShouldAccumulateTotalHoursPlayed()
    {
        // Arrange
        var patchClient = ApplicationFactory.CreateClient();
        var postClient = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Bob");
        var createResponse = await postClient.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var tester = createdBody.Data;

        var firstUpdateRequest = new UpdatePlaytimeRequest(HoursPlayed: 1.5);
        var secondUpdateRequest = new UpdatePlaytimeRequest(HoursPlayed: 2.0);
        var requestUri = $"/api/testers/{tester.AccessKey}/playtime";
        double expectedHoursPlayed = 3.5f;

        // Act
        await patchClient.PatchAsJsonAsync(requestUri, firstUpdateRequest);
        await patchClient.PatchAsJsonAsync(requestUri, secondUpdateRequest);

        // Assert
        var updatedTester = await FirstOrDefaultAsync<Tester>(t => t.AccessKey == tester.AccessKey);
        updatedTester.TotalHoursPlayed.Should().Be(expectedHoursPlayed);
    }

    [Test]
    public async Task Patch_WhenTesterNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var patchClient = ApplicationFactory.CreateClient();
        var postClient = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Bob");
        await postClient.PostAsJsonAsync("/api/testers", createRequest);

        var fakeAccessKey = Guid.NewGuid().ToString();
        var updateRequest = new UpdatePlaytimeRequest(HoursPlayed: 1.0);
        var requestUri = $"/api/testers/{fakeAccessKey}/playtime";

        // Act
        var response = await patchClient.PatchAsJsonAsync(requestUri, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Patch_WhenHoursPlayedNegative_ShouldReturnBadRequest()
    {
        // Arrange
        var patchClient = ApplicationFactory.CreateClient();
        var postClient = CreateHttpClientWithApiKey();
        var createRequest = new CreateTesterRequest(Name: "Charlie");
        var createResponse = await postClient.PostAsJsonAsync("/api/testers", createRequest);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<Result<CreateTesterResponse>>();
        var tester = createdBody.Data;

        var updateRequest = new UpdatePlaytimeRequest(HoursPlayed: -1);
        var requestUri = $"/api/testers/{tester.AccessKey}/playtime";

        // Act
        var response = await patchClient.PatchAsJsonAsync(requestUri, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
