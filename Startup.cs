using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PikaStatus.Services;

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
            services.AddServerSideBlazor();
            services.AddSingleton<MessageService>();
            services.AddResponseCaching();
            services.AddResponseCompression();
            services.AddCors();
            services.AddHsts(o =>
            {
            });
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
            app.UseFileServer();
            app.UseRouting();
            app.UseResponseCaching();
            app.UseResponseCompression();
            app.UseCors(options =>
            {
                options.AllowCredentials();
                options.AllowAnyHeader();
                options.WithMethods("GET");
                options.WithOrigins("https://localhost:5001", 
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