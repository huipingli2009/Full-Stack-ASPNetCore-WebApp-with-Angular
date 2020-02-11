using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.api;
using org.cchmc.pho.api.Controllers;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.identity.Models;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.unittest.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace org.cchmc.pho.unittest.controllertests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private UserController _userController;
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<FakeUserManager> _mockUserManager;
        private IMapper _mapper;
        private UserManager<User> _userManager;
        private string _userName = "someUserHere";
        private string _password = "SomePassword1!";
        private string _email = "some@example.com";
        private string _first = "some";
        private string _last = "user";

        public UserControllerTests()
        {
            Setup(new TestFixture<Startup>());
        }

        public void Setup(TestFixture<Startup> testFixture)
        {
            _userManager = (UserManager<User>)testFixture.Server.Host.Services.GetService(typeof(UserManager<User>));
        }

        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
            try
            {
                var x = Task.Run(async () => { await _userManager.CreateAsync(new User()
                {
                    Email = _email,
                    FirstName = _first,
                    LastName = _last,
                    UserName = _userName
                }, _password); });
                while(!x.IsCompleted) { }
            }
            catch (Exception ex)
            {

            }
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
            });
            _mapper = config.CreateMapper();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockUserManager = new Mock<FakeUserManager>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if(_userManager != null)
            {
                var x = Task.Run(async () =>
                {
                    var user = await _userManager.FindByNameAsync(_userName);
                    if (user != null)
                    {
                        await _userManager.DeleteAsync(new User()
                        {
                            Id = user.Id
                        });
                    }
                });
                while (!x.IsCompleted) { }
            }
        }

        [TestMethod]
        public async Task GetUserById_WithMock_Success()
        {
            // setup
            var userId = "someUserHere";
            var email = "some@example.com";
            var first = "first";
            var last = "last";
            _mockUserManager.Setup(p => p.FindByNameAsync(userId)).Returns(Task.FromResult(new User()
            {
                Email = email,
                FirstName = first,
                LastName = last,
                UserName = userId
            })).Verifiable();
            _userController = new UserController(_mockLogger.Object, _mapper, _mockUserManager.Object);

            // execute
            var result = await _userController.GetUser(userId) as ObjectResult;
            var resultUser = result.Value as UserViewModel;

            // assert
            Assert.AreEqual(email, resultUser.Email);
            Assert.AreEqual(first, resultUser.FirstName);
            Assert.AreEqual(last, resultUser.LastName);
            Assert.AreEqual(userId, resultUser.UserName);
        }

        [TestMethod]
        public async Task GetUserById_WithRealIdentity_Success()
        {
            _userController = new UserController(_mockLogger.Object, _mapper, _userManager);

            // execute
            var result = await _userController.GetUser(_userName) as ObjectResult;
            var resultUser = result.Value as UserViewModel;

            // assert
            Assert.AreEqual(_email, resultUser.Email);
            Assert.AreEqual(_first, resultUser.FirstName);
            Assert.AreEqual(_last, resultUser.LastName);
            Assert.AreEqual(_userName, resultUser.UserName);
        }
    }

    [ExcludeFromCodeCoverage]
    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<User>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<User>>().Object,
                  new IUserValidator<User>[0],
                  new IPasswordValidator<User>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<User>>>().Object)
        { }
    }

    [ExcludeFromCodeCoverage]
    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager()
            : base(new Mock<FakeUserManager>().Object,
                  new HttpContextAccessor(),
                  new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<User>>>().Object,
                  new Mock<IAuthenticationSchemeProvider>().Object,
                  new Mock<IUserConfirmation<User>>().Object)
        { }
    }
}
