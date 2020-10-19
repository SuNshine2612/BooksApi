using System;
using BooksApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using AppContext = BooksWebApp.Helper.AppContext;

namespace BooksWebApp
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(); // Very important to use return Json(dynamic type)
                

            // HttpSession
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.Cookie.Name = StaticVar.CookiesName;
                options.IdleTimeout = TimeSpan.FromSeconds(28800);
                options.Cookie.IsEssential = true;
            });
            services.AddDistributedMemoryCache();

            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1
            // Add response cache
            services.AddResponseCaching();

            // Authenticate use CookieAuthentication 
            services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", config =>
                {
                    config.Cookie.Name = StaticVar.CookiesAuthenticate;
                    config.LoginPath = "/UserTest/Login";
                    config.AccessDeniedPath = "/UserTest/Login";
                    config.SlidingExpiration = false;
                    config.ExpireTimeSpan = TimeSpan.FromDays(2);

                    // set lifetime = lifetine token expired !!
                });
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
            // Cache to save static files
            // https://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                    ctx.Context.Response.Headers[HeaderNames.Expires] = DateTime.UtcNow.AddYears(1).ToString("R");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Http Session
            app.UseSession();
            AppContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            // Http Response cache
            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1
            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new CacheControlHeaderValue()
                    {
                        Public = bool.Parse(Configuration["ResponseCache:Headers:Public"]),
                        MaxAge = TimeSpan.FromSeconds(double.Parse(Configuration["ResponseCache:Headers:MaxAge"]))
                    };
                context.Response.Headers[HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
