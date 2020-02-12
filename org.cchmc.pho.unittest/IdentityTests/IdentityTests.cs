using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.api.Controllers;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.IdentityTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class IdentityTests
    {
        private static UserManager<User> _userManager;
        private static IdentityDataContext _context;
        private string userName = "someone";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\org.cchmc.pho.api\\");
            configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            configBuilder.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: false);
            var configuration = configBuilder.Build();
            var identityConnString = configuration.GetConnectionString("pho-identity");
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration)
                    .AddLogging()
                    .AddIdentityServices(identityConnString);
                    
            var provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<IdentityDataContext>();
            var userManager = provider.GetRequiredService<UserManager<User>>();
            Setup(context, userManager);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            _context.Database.CloseConnection();
        }

        private static void Setup(IdentityDataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            // performs all migrations on the database, so if you haven't installed it, it will be created
            _context.Database.Migrate();
            _context.Database.OpenConnection();
        }

        [TestInitialize]
        public async Task Initialize()
        {
            await Cleanup();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var u in users)
            {
                try
                {
                    await _userManager.DeleteAsync(u);
                }
                catch (Exception ex)
                {

                }
            }
        }

        [TestMethod]
        public async Task InsertUser()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            var password = "SomePassword!1";

            var result = await _userManager.CreateAsync(user, password);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task DeleteUser()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            var password = "SomePassword!1";
            await _userManager.CreateAsync(user, password);

            var userToDelete = await _userManager.FindByNameAsync(userName);

            var result = await _userManager.DeleteAsync(userToDelete);
            Assert.IsTrue(result.Succeeded);

            userToDelete = await _userManager.FindByNameAsync(userName);
            Assert.IsNull(userToDelete);
        }

        [TestMethod]
        public async Task AddRoleToUser()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            var password = "SomePassword!1";

            var result = await _userManager.CreateAsync(user, password);
            user = await _userManager.FindByNameAsync(userName);

            result = await _userManager.AddToRoleAsync(user, "Role1");
            Assert.IsTrue(result.Succeeded);
            var roles = await _userManager.GetRolesAsync(user);
            Assert.IsTrue(roles.Contains("Role1"));
            var users = await _userManager.GetUsersInRoleAsync("Role1");
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(userName, users[0].UserName);
        }
    }
}
