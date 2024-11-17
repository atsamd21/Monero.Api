using Microsoft.Extensions.Options;
using Monero.Api.Attributes;
using Monero.Models;

namespace Monero.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOptions<MoneroSettings> settings)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<ApiKeyAuthorizeAttribute>() is null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey) || apiKey != settings.Value.APIKey)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _next(context);
    }
}
