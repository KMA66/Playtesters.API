using Playtesters.API.Services;

namespace Playtesters.API.Tests.Common;

public class FakeClientIpProvider : IClientIpProvider
{
    public string GetClientIp() => "127.0.0.1";
}
