using Microsoft.AspNetCore.Builder;
using PikaStatus.Middlewares;

namespace PikaStatus.Extensions;

public static class MapJwtClaimsToIdentityMiddlewareExtensions
{
    public static IApplicationBuilder UseMapJwtClaimsToIdentity(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MapJwtClaimsToIdentityMiddleware>();
    }
}