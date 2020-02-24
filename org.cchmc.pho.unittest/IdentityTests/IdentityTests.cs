using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.cchmc.pho.identity;
using org.cchmc.pho.identity.Extensions;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.IdentityTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class IdentityTests
    {
        private static UserManager<User> _userManager;
        private static SignInManager<User> _signInManager;
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
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration)
                    .AddLogging()
                    .AddIdentityServices(configuration);
                    
            var provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<IdentityDataContext>();
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var signInManager = provider.GetRequiredService<SignInManager<User>>();
            Setup(context, userManager, signInManager);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            _context.Database.CloseConnection();
        }

        private static void Setup(IdentityDataContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

        // This test is for running the migration that deploys to your local environment, per the readme.
        [Ignore]
        [TestMethod]
        public async Task DoNothing()
        {

        }

        [Ignore("These tests are for experimenting with Identity features only.")]
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

        [Ignore("These tests are for experimenting with Identity features only.")]
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

        [Ignore("These tests are for experimenting with Identity features only.")]
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

        [Ignore("These tests are for experimenting with Identity features only.")]
        [TestMethod]
        public async Task Login()
        {
            // create user
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            var password = "SomePassword!1";
            var result = await _userManager.CreateAsync(user, password);

            // add role to user
            user = await _userManager.FindByNameAsync(userName);
            result = await _userManager.AddToRoleAsync(user, "Role1");
            var roles = new List<string>(await _userManager.GetRolesAsync(user));

            // check the password
            var passwordResult = await _userManager.CheckPasswordAsync(user, password);
            Assert.IsTrue(passwordResult);

            // get a claim
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            roles.ForEach(p => identity.AddClaim(new Claim(ClaimTypes.Role, p)));

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
        }
    }
}
