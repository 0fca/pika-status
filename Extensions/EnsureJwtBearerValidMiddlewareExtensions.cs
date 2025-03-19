using Microsoft.AspNetCore.Builder;
using PikaStatus.Middlewares;

namespace PikaStatus.Extensions;

public static class EnsureJwtBearerValidMiddlewareExtensions
{
    public static IApplicationBuilder UseEnsureJwtBearerValid(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EnsureJwtBearerValidMiddleware>();
    }
}