using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using PikaStatus.Enums;

namespace PikaStatus.Middlewares.AuthenticationStrategies;

public class UserByTokenInQuery(string token)
{
    public ClaimsPrincipal Execute()
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var jwst = jsonToken as JwtSecurityToken;
        var user = new ClaimsPrincipal();
        var identity = new ClaimsIdentity();
        jwst!.Claims.ToList().ForEach(c =>
        {
            if (c.Type is not (KeycloakClaimTypes.Roles or KeycloakClaimTypes.RealmAccess)) return;
            var roleClaimTypeName = identity.RoleClaimType;
            const string roleClaimStandardName = ClaimTypes.Role;
            var roles = JsonSerializer.Deserialize<Dictionary<string, IList<string>>>(c.Value)["roles"];
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(roleClaimTypeName, role));
                identity.AddClaim(new Claim(roleClaimStandardName, role));
            }
        });
        user.AddIdentity(identity);
        return user;
    }
}