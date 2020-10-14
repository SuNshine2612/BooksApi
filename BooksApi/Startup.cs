using System;

using System.IdentityModel.Tokens.Jwt;

using System.Text;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace BooksApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigFile.SecretKey = configuration.GetSection("TokenAuthentication:SecretKey").Value;
            ConfigFile.Issuer = configuration.GetSection("TokenAuthentication:Issuer").Value;
            ConfigFile.Audience = configuration.GetSection("TokenAuthentication:Audience").Value;

            ConfigFile.ConnectionString = configuration.GetSection("MongoConnection:ConnectionString").Value;
            ConfigFile.Database = configuration.GetSection("MongoConnection:Database").Value;

            ConfigFile.MyMongoClient = new MongoClient(ConfigFile.ConnectionString);
            ConfigFile.MyMongoDatabase = ConfigFile.MyMongoClient.GetDatabase(ConfigFile.Database);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //https://www.yogihosting.com/aspnet-core-enable-cors/
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddSingleton(typeof(GenericService<>));

            //cấu hình cho authenticate
            
            //Phần này là để định cấu hình Cơ chế xác thực mà chúng ta sẽ sử dụng
            services.AddAuthentication(auth =>
            {
                //yêu cầu ASP.NET Core sử dụng Xác thực Bearer JWT.
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(token =>
            {
                token.RequireHttpsMetadata = true;
                token.SaveToken = true;
                token.TokenValidationParameters = new TokenValidationParameters
                {
                    //Same Secret key will be used while creating the token
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ConfigFile.SecretKey)),
                    ValidateIssuerSigningKey = true,

                    //Usually, this is your application base URL
                    ValidIssuer = ConfigFile.Issuer,
                    ValidateIssuer = true,

                    //Here, we are creating and using JWT within the same application.
                    //In this case, base URL is fine.
                    //If the JWT is created using a web service, then this would be the consumer URL.
                    ValidAudience = ConfigFile.Audience,
                    ValidateAudience = true,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                };
                token.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            var claimPrincipal = context.Principal;
                            //set biến global dùng chung
                            UserClaim.Token = accessToken;
                            UserClaim.ObjectId = claimPrincipal.FindFirst(StaticVar.ClaimObjectId).Value;
                            UserClaim.UserId = claimPrincipal.FindFirst(StaticVar.ClaimCode).Value;
                            UserClaim.FullName = claimPrincipal.FindFirst(StaticVar.ClaimName).Value;
                            //UserClaim.CompanyId = claimPrincipal.FindFirst("CompanyId").Value;
                            //UserClaim.DeviceId = claimPrincipal.FindFirst("SerialNumber").Value ?? string.Empty;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        UserClaim.Reset();
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Demo Books API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("http://www.unilogistics.vn/"),
                    Contact = new OpenApiContact
                    {
                        Name = "SuNshine",
                        Email = string.Empty,
                        Url = new Uri("http://www.unilogistics.vn/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("http://www.unilogistics.vn/"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                /*var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);*/
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowMyOrigin");


            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API V1");
            });
        }
    }
}
