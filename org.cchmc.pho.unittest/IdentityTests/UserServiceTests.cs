using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.identity;
using org.cchmc.pho.identity.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.IdentityTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<IOptions<JwtAuthentication>> _mockJwtAuthentication;

        protected static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            return mgr;
        }

        protected static Mock<SignInManager<User>> MockSignInManager(Mock<UserManager<User>> mockUserManager)
        {
            return new Mock<SignInManager<User>>(mockUserManager.Object,
                 new Mock<IHttpContextAccessor>().Object,
                 new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                 new Mock<IOptions<IdentityOptions>>().Object,
                 new Mock<ILogger<SignInManager<User>>>().Object,
                 new Mock<IAuthenticationSchemeProvider>().Object,
                 new Mock<IUserConfirmation<User>>().Object);
        }

        [TestInitialize]
        public void Initialize()
        {
            _mockUserManager = MockUserManager();
            _mockSignInManager = MockSignInManager(_mockUserManager);

            _mockJwtAuthentication = new Mock<IOptions<JwtAuthentication>>();
            _mockJwtAuthentication.Setup(x => x.Value).Returns(new JwtAuthentication()
            {
                SecurityKey = "1234567890123456789012345678901234567890123456789012345678901234567890",
                ValidAudience = "PHO",
                ValidIssuer = "PHO",
                TokenExpirationInHours = 5
            });
            _userService = new UserService("", _mockJwtAuthentication.Object, _mockUserManager.Object, _mockSignInManager.Object);
        }


        [TestMethod]
        public async Task Authenticate_NoSuchUser_ReturnsNull()
        {
            // setup
            var userName = "myUser";
            var password = "mypassword";
            _mockUserManager.Setup(p => p.FindByNameAsync(userName)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userService.Authenticate(userName, password);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task Authenticate_BadPassword_ReturnsNull()
        {
            // setup
            var userName = "myUser";
            var password = "mypassword";
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            _mockUserManager.Setup(p => p.FindByNameAsync(userName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.CheckPasswordAsync(It.Is<User>(x => x.Email == user.Email
                                                                           && x.FirstName == user.FirstName
                                                                           && x.LastName == user.LastName
                                                                           && x.UserName == user.UserName),
                                                             password)).Returns(Task.FromResult(false)).Verifiable();

            // execute
            var result = await _userService.Authenticate(userName, password);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.GetRolesAsync(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task Authenticate_GoodPassword_ReturnsUserWithToken()
        {
            // setup
            var userName = "myUser";
            var password = "mypassword";
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName
            };
            _mockUserManager.Setup(p => p.FindByNameAsync(userName))
                .Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.CheckPasswordAsync(It.Is<User>(x => x.Email == user.Email
                                                                           && x.FirstName == user.FirstName
                                                                           && x.LastName == user.LastName
                                                                           && x.UserName == user.UserName),
                                                             password))
                .Returns(Task.FromResult(true)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user))
                .Returns(Task.FromResult((IList<string>)new List<string>() { "ROLE1", "ROLE2" })).Verifiable();

            // execute
            var result = await _userService.Authenticate(userName, password);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.AreNotEqual("", result.Token);
            _mockUserManager.Verify();
        }
    }
}
