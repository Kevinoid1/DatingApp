using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Models;
using DatingApp.Repositories;
using DatingApp.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp
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
            services.AddDbContext<DatingAppContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSingleton<PresenceTracker>();

            //service filter
            services.AddScoped<LogUserLastActive>();

            //cloudinary
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            //automapper
            services.AddAutoMapper(typeof(IDatingRepository).Assembly);

            //cors
            services.AddCors();

            #region identity services 
            //use addIdentityCore since it is not an MVC and i don't need the identity pages. AddIdentity is the default for MVC that does this
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<DatingAppContext>();

            #endregion

            //authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                //for signalR,  we send token as query string and tell the application how to retrieve the token.
                //token is sent as query string because websockets does not support authorization header as of now
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            //authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyEnum.AdminRole, pol => pol.RequireRole(RolesEnum.Admin));
                options.AddPolicy(PolicyEnum.ModeratorRole, pol => pol.RequireRole(RolesEnum.Admin, RolesEnum.Moderator));
            });
            
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //method 1 -- handling exceptions with a global exception handler
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{

            //    // app.UseExceptionHandler("/Error");
            //    app.UseExceptionHandler(builder =>
            //    {
            //        builder.Run(async context =>
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //            var error = context.Features.Get<IExceptionHandlerFeature>();
            //            if (error != null)
            //            {
            //                context.Response.AddApplicationErrors(error.Error.Message);
            //                await context.Response.WriteAsync(error.Error.Message);
            //            }
            //        });
            //    });
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //method 2 -- global exception handling wrote a middleware class to handle errors globally
            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            //added allow credentials for signalr mode of sending token
            app.UseCors(o => o.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
