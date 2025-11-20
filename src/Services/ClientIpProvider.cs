namespace Playtesters.API.Services;

public interface IClientIpProvider
{
    string GetClientIp();
}

public class ClientIpProvider(
    IHttpContextAccessor httpContextAccessor) : IClientIpProvider
{
    public string GetClientIp()
    {
        var context = httpContextAccessor.HttpContext;
        var headers = context.Request.Headers;

        // Cloudflare header (used by Railway and most managed hosting providers)
        // This is the most reliable way to get the original client IP.
        var cfIp = headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(cfIp))
            return cfIp;

        // Standard reverse proxy header
        // Can contain multiple IPs: "client, proxy1, proxy2"
        // The first entry is always the real client IP.
        var xForwardedFor = headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(xForwardedFor))
            return xForwardedFor.Split(',')[0].Trim();

        // Fallback: direct connection IP (usually the proxy IP when behind a load balancer).
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
