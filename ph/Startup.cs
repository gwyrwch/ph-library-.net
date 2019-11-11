using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ph.Data;
using ph.Models;
using ph.RouteConstraints;

namespace ph
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });
            
            services.AddIdentity<User, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.Configure<IdentityOptions>(options =>
            {
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
//                options.User.RequireUniqueEmail = false;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Auth/Login";
                options.SlidingExpiration = true;
            }); 

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU"),
                };
                options.DefaultRequestCulture = new RequestCulture("ru-RU");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();;
            
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabaseInitializer dbInitializer)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ru-RU")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            dbInitializer.Initialize();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "createPet",
                    "Auth/CreatePet",
                    new {controller = "Auth", action = "CreatePet"}
                );

                routes.MapRoute(
                    "createUser",
                    "Auth/CreateUser",
                    new {controller = "Auth", action = "CreateUser"}
                );
                
                routes.MapRoute(
                    "feed",
                    "Home/Feed/{type:int}",
                    new {controller = "Home", action = "Feed", type = -1},
                     new
                    {
                        type = new CompositeRouteConstraint(new IRouteConstraint[] {
                                new RangeRouteConstraint(-1, 6),
                                new IntRouteConstraint()
                        })
                    });
                
                routes.MapRoute(
                    "likeEvent",
                    "Home/LikeEvent/{postId}",
                    new {controller = "Home", action = "LikeEvent"}
                );
                
                routes.MapRoute(
                    "changelanguage",
                    "Home/SetLanguage/{culture}",
                    new {controller = "Home", action = "SetLanguage"}
                );

                routes.MapRoute(
                    "home",
                    "Home/{action}",
                    new {controller = "Home"},
                    new
                    {
                        action = new ActionConstraint
                        {
                            ActionsPossible = new List<string> {"profile", "settings", "createpost", "index", "logout"}
                        }
                    }
                );

                routes.MapRoute(
                    "admin",
                    "Admin/Index",
                    new {controller = "Admin", action = "Index"}
                );
                routes.MapRoute(
                    "admin",
                    "Admin/DeleteUser",
                    new {controller = "Admin", action = "DeleteUser"}
                );

                routes.MapRoute(
                    "default",
                    "{controller}/{action}",
                    new{controller="Auth", action="Login"},
                    new{controller="Auth", action="Login"}
                );
                
            });
        }
    }
}
