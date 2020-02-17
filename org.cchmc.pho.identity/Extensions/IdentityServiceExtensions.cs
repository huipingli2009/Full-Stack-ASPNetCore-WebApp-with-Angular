using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using org.cchmc.pho.identity.Configuration;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace org.cchmc.pho.identity.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class IdentityServiceExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtAuthentication>(opts => configuration.GetSection("JwtAuthentication"));
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.AddOptions<JwtAuthentication>()
                        .Bind(configuration.GetSection("JwtAuthentication"))
                        .ValidateDataAnnotations() //todo 
                        .Validate(c =>
                        {
                            return true;
                        }, "failure message");
            services.AddDbContext<IdentityDataContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("phoidentity"), optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly("org.cchmc.pho.identity");
                });
            });
            services.AddIdentity<User, IdentityRole>(opts =>
                {
                    // TODO: Address the correct values for these
                    opts.Password.RequiredLength = 8;
                    opts.Password.RequireNonAlphanumeric = true;
                    opts.Password.RequireLowercase = true;
                    opts.Password.RequireUppercase = true;
                    opts.Password.RequireDigit = true;

                    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    opts.Lockout.MaxFailedAccessAttempts = 5;
                    opts.Lockout.AllowedForNewUsers = true;

                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>()
                .AddRoleManager<RoleManager<IdentityRole>>() // this might be unnecessary but included so we know we're using role manager specifically
                .AddDefaultTokenProviders();

            services.AddTransient<IUserService, UserService>();
        }

        public static void ConfigureIdentityServices<TStartup>(this IApplicationBuilder app, ILogger<TStartup> logger)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<IdentityDataContext>())
                {
                    try
                    {
                        /* TODO: Microsoft recommends -NOT- doing this in a production environment, especially in a distributed setup.
                           Investigate doing this via the deployment pipeline instead.
                        */
                        var migrations = context.Database.GetPendingMigrations();
                        if (migrations.Any())
                        {
                            logger.LogInformation($"Applying ${migrations.Count()} migrations to Identity database.");
                            context.Database.Migrate();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                }
            }
        }
    }
}
