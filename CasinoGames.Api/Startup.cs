using CasinoGames.Api.Data;
using CasinoGames.Api.Logic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CasinoGames.Api
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
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin());
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    static Task StopRedirect(Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions> context)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    options.Events.OnRedirectToAccessDenied = StopRedirect;
                    options.Events.OnRedirectToLogin = StopRedirect;

                    options.Cookie.SameSite = SameSiteMode.Lax;

                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;

                    options.Validate();
                });

            services
                .AddControllers();

            services.AddDbContext<UserGameContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CasinoUser")));
            services.AddDbContext<AdminGameContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CasinoAdmin")));

            services
                .AddRoundRobin<IJackpotProvider>(ServiceLifetime.Scoped, ServiceLifetime.Transient)
                .AddImplementation<JackpotProviderA>()
                .AddImplementation<JackpotProviderB>();

            // We will only resolve IAdminJackpotProvider is user is authenticated
            services.AddTransient(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>();
                if ((context?.HttpContext?.User?.Identity?.IsAuthenticated).GetValueOrDefault())
                {
                    return provider.GetRequiredService<IJackpotProvider>() as IAdminJackpotProvider;
                }
                return default;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/info");
                endpoints.MapControllers();
            });
        }
    }
}