using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Pika.Domain.Security;
using PikaStatus.Enums;

namespace PikaStatus.Middlewares;

public class MapJwtClaimsToIdentityMiddleware
{
    private readonly RequestDelegate _next;

    public MapJwtClaimsToIdentityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies[".AspNet.Identity"];
        if (string.IsNullOrEmpty(token))
        {
            context.User.AddClaim(ClaimTypes.Role, RoleString.User);
            await _next(context);
            return;
        }
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var jwst = jsonToken as JwtSecurityToken;
        jwst!.Claims.ToList().ForEach(c =>
        {
            if (c.Type is not (KeycloakClaimTypes.Roles or KeycloakClaimTypes.RealmAccess)) return;
            var roleClaimTypeName = context.User.Identities.First().RoleClaimType;
            const string roleClaimStandardName = ClaimTypes.Role;
            var roles = JsonSerializer.Deserialize<Dictionary<string, IList<string>>>(c.Value)["roles"];
            foreach (var role in roles)
            {
                context.User.AddClaim(roleClaimTypeName, role);
                context.User.AddClaim(roleClaimStandardName, role);
            }
        });
        await _next(context);
    }
}