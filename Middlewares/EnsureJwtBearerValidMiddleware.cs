using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PikaStatus.Middlewares;

public class EnsureJwtBearerValidMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public EnsureJwtBearerValidMiddleware(RequestDelegate next, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _next = next;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies[".AspNet.Identity"];
        if (string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var jwst = jsonToken as JwtSecurityToken;
        if (jwst == null)
        {
            await _next(context);
            return;
        }

        var validTo = jwst.ValidTo.ToLocalTime();
        var localNow = DateTime.Now.ToLocalTime();

        if (validTo <= localNow)
        {
            var refreshToken = context.Request.Cookies[".AspNet.Identity.Refresh"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Attempt to refresh token using the refresh token cookie
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var refreshEndpoint = _tokenResponse_endpoint_config(context);

                    if (!string.IsNullOrEmpty(refreshEndpoint))
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, refreshEndpoint);
                        var content = new FormUrlEncodedContent(new[]
                        {
                             new KeyValuePair<string, string>("grant_type", "refresh_token"),
                             new KeyValuePair<string, string>("client_id", _configuration["Keycloak:ClientId"] ?? string.Empty),
                             new KeyValuePair<string, string>("client_secret", _configuration["Keycloak:ClientSecret"] ?? string.Empty),
                             new KeyValuePair<string, string>("refresh_token", refreshToken)
                         });
                        request.Content = content;

                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                            {
                                context.Response.Cookies.Append(".AspNet.Identity", tokenResponse.AccessToken, new CookieOptions
                                {
                                    Path = "/",
                                    Domain = _configuration["Keycloak:CookieDomain"],
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
                                });
                            }
                        }
                    }
                }
                catch { /* Fallback to 401 */ }
            }

            context.Response.Cookies.Delete(".AspNet.Identity", new CookieOptions
            {
                Path = "/",
                Domain = _configuration.GetSection("Keycloak")["CookieDomain"]
            });
            context.Response.StatusCode = 401;
        }
        await _next(context);
    }

    private string? _tokenResponse_endpoint_config(HttpContext context)
    {
        return _configuration["Keycloak:RefreshEndpoint"];
    }
}
