using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PikaStatus.Middlewares;

public class ReturnUrlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ReturnUrlMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Response.StatusCode is 401 or 403)
        {
            context.Response.Redirect($"/Identity/Gateway/Login?returnUrl={context.Request.Path}", true);
        } 
    }
}