using Playtesters.API.Services;

namespace Playtesters.API.Tests.Common;

public class FakeIpGeoLocationService : IIpGeoLocationService
{
    public async Task<GeoLocationResponse> GetLocationAsync(string ipAddress)
    {
        var response = new GeoLocationResponse(Country: "Colombia", City: "Medellin");
        return await Task.FromResult(response);
    }
}
