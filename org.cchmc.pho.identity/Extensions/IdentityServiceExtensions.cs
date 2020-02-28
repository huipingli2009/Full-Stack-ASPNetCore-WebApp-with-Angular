using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using org.cchmc.pho.identity.Configuration;
using org.cchmc.pho.identity.EntityModels;
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
                        .ValidateDataAnnotations() // TODO
                        .Validate(c =>
                        {
                            return true;
                        }, "Failed to validate JWT Authentication options.");

            // Looking at the PHO database for login information, ideally as a temporary measure
            services.AddDbContext<PHOIdentityContext>(options => options.UseSqlServer(configuration.GetConnectionString("phodb")));

            services.Configure<PasswordHasherOptions>(options =>
                {
                    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
                    options.IterationCount = 10000;
                });
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddTransient<IUserService, PhoUserService>();
        }

        public static AuthorizationPolicy BuildAuthorizationPolicy(this IConfiguration configuration)
        {
            var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

            return policy;
        }
    }
}
