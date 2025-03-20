using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using PikaStatus.Extensions;
using PikaStatus.Services;
using System;

namespace PikaStatus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor(c =>
            {
                c.DetailedErrors = true;
            });
            services.AddHealthChecks();
            services.AddSingleton<MessageService>();
            services.AddResponseCaching();
            services.AddCors();
            services.AddCascadingAuthenticationState();
            services.AddOpenIddict()
                .AddClient(o =>
                {
                    o.DisableTokenStorage();
                    o.AllowClientCredentialsFlow();
                    o.AllowRefreshTokenFlow();
                    o.AddRegistration(new OpenIddictClientRegistration
                    {
                        RegistrationId = "pika-core",
                        ClientId = Configuration.GetSection("Keycloak")["ClientId"],
                        ClientSecret = Configuration.GetSection("Keycloak")["ClientSecret"],
                        Issuer = new Uri(Configuration.GetSection("Keycloak")["Authority"], UriKind.Absolute)
                    });
                    o.UseSystemNetHttp()
                        .SetProductInformation(typeof(Program).Assembly);
                }) 
                .AddValidation(o =>
                {
                    o.SetIssuer(Configuration.GetSection("Keycloak")["Authority"]);
                    o.UseDataProtection();
                    o.UseIntrospection()
                        .SetClientId(Configuration.GetSection("Keycloak")["ClientId"])
                        .SetClientSecret(
                            Configuration.GetSection("Keycloak")["ClientSecret"]);
                    o.UseSystemNetHttp();
                    o.UseAspNetCore();
                });
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });
            services.AddAuthorizationCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseResponseCaching();
            app.UseOiddictAuthenticationCookieSupport();
            app.UseMapJwtClaimsToIdentity();
            app.UseEnsureJwtBearerValid();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/Health");

            app.UseCors(options =>
            {
                options.AllowCredentials();
                options.AllowAnyHeader();
                options.WithMethods("GET");
                options.WithOrigins("https://core.cloud.localhost:5001",
                    "http://core.cloud.localhost:5000",
                    "https://core.lukas-bownik.net",
                    "https://dev-core.lukas-bownik.net",
                    "https://me.lukas-bownik.net"
                    );
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub(o =>
                {
                    o.Transports = HttpTransportType.ServerSentEvents | HttpTransportType.LongPolling;
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}