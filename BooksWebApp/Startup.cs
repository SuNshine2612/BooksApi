using System;
using BooksApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Http Session
            app.UseSession();
            AppContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
