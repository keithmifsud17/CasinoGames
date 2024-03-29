using CasinoGames.Website.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace CasinoGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            services.AddTransient<CookieHandler>();

            services
                .AddHttpClient<IGameHttpClient, GameHttpClient>(client =>
                {
                    client.BaseAddress = new System.Uri(Configuration.GetValue<string>("Api"));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false })
                .AddHttpMessageHandler<CookieHandler>();

            services
                .AddHttpClient<IAuthHttpClient, AuthHttpClient>(client =>
                {
                    client.BaseAddress = new System.Uri(Configuration.GetValue<string>("Api"));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false })
                .AddHttpMessageHandler<CookieHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}