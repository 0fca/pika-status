using Microsoft.AspNetCore.Builder;
using PikaStatus.Middlewares;

namespace PikaStatus.Extensions;

public static class OiddictAuthenticationCookieSupportMiddlewareExtensions
{
    public static IApplicationBuilder UseOiddictAuthenticationCookieSupport(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OiddictAuthenticationCookieSupportMiddleware>();
    }
}