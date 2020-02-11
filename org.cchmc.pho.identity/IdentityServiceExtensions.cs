using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.identity
{
    public static class IdentityServiceExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IdentityDataContext>(opts =>
                opts.UseSqlServer(connectionString, optionsBuilder =>
                    optionsBuilder.MigrationsAssembly("org.cchmc.pho.identity")));
            services.AddIdentity<User, IdentityRole>(opts =>
                {
                    opts.Password.RequiredLength = 8;
                    opts.Password.RequireNonAlphanumeric = true;
                    opts.Password.RequireLowercase = true;
                    opts.Password.RequireUppercase = true;
                    opts.Password.RequireDigit = true;
                })
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();
        }
    }
}
