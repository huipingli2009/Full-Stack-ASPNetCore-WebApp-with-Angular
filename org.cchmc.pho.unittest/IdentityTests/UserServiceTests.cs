using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.identity;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.IdentityTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IOptions<JwtAuthentication>> _mockJwtAuthentication;
        private Mock<ILogger<UserService>> _mockLogger;
        private Mock<IPasswordValidator<User>> _mockPasswordValidator;

        protected static Mock<UserManager<User>> MockUserManager(Mock<IPasswordValidator<User>> mockPasswordValidator)
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(mockPasswordValidator.Object);
            return mgr;
        }

        // Not removing this just yet, I think we'll need it as features expand
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

        protected static Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);
        }

        [TestInitialize]
        public void Initialize()
        {
            _mockPasswordValidator = new Mock<IPasswordValidator<User>>();
            _mockUserManager = MockUserManager(_mockPasswordValidator);

            _mockJwtAuthentication = new Mock<IOptions<JwtAuthentication>>();
            _mockJwtAuthentication.Setup(x => x.Value).Returns(new JwtAuthentication()
            {
                SecurityKey = "1234567890123456789012345678901234567890123456789012345678901234567890",
                ValidAudience = "PHO",
                ValidIssuer = "PHO",
                TokenExpirationInHours = 5
            });
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockRoleManager = MockRoleManager();
            _userService = new UserService(_mockJwtAuthentication.Object, _mockUserManager.Object, _mockRoleManager.Object, _mockLogger.Object);
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

        [TestMethod]
        public async Task Authenticate_IdentityExceptionIsCaught_ReturnsNull()
        {
            // setup
            var userName = "myUser";
            var password = "mypassword";
            _mockUserManager.Setup(p => p.FindByNameAsync(userName)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.Authenticate(userName, password);

            // assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByEmail_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            _mockUserManager.Setup(p => p.FindByEmailAsync(user.Email)).Returns(Task.FromResult(user)).Verifiable();

            // execute
            var result = await _userService.GetUserByEmail(user.Email);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.UserName, result.UserName);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetUserByEmail_NoUser_ReturnsNull()
        {
            var someemail = "asdfasdfs";
            _mockUserManager.Setup(p => p.FindByEmailAsync(someemail)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userService.GetUserByEmail(someemail);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetUserByEmail_ExceptionCaught_ReturnsNull()
        {
            var someemail = "asdfasdfs";
            _mockUserManager.Setup(p => p.FindByEmailAsync(someemail)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.GetUserByEmail(someemail);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetUserByUserName_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();

            // execute
            var result = await _userService.GetUserByUserName(user.UserName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.UserName, result.UserName);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetUserByUserName_NoUser_ReturnsNull()
        {
            var someName = "asdfasdfs";
            _mockUserManager.Setup(p => p.FindByNameAsync(someName)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userService.GetUserByUserName(someName);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetUserByUserName_ExceptionCaught_ReturnsNull()
        {
            var someName = "asdfasdfs";
            _mockUserManager.Setup(p => p.FindByNameAsync(someName)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.GetUserByUserName(someName);

            // assert
            Assert.IsNull(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetRoles_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var listOfRoles = new List<string> { "ROLE1", "ROLE2" };

            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)listOfRoles)).Verifiable();

            // execute
            var result = await _userService.GetRoles(user.UserName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            result.ForEach(p => Assert.IsTrue(listOfRoles.Contains(p)));
            listOfRoles.ForEach(p => Assert.IsTrue(result.Contains(p)));
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetRoles_NoUser_ReturnsEmptyList()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            
            // execute
            var result = await _userService.GetRoles(user.UserName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.GetRolesAsync(It.IsAny<User>()), Times.Never);

        }

        [TestMethod]
        public async Task GetRoles_NoRoles_ReturnsEmptyList()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var listOfRoles = new List<string>();

            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)listOfRoles)).Verifiable();

            // execute
            var result = await _userService.GetRoles(user.UserName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task GetRoles_ExceptionCaught_ReturnsEmptyList()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var listOfRoles = new List<string>();

            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.GetRoles(user.UserName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public void GetRolesFromClaims_FiltersOutRoles()
        {
            // setup
            var userName = "someUser";
            var firstName = "first";
            var lastName = "last";
            var id = "dfsdvastjy";
            var roles = new List<string> { "ROLE1", "ROLE2" };
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, firstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, lastName));
            roles.ForEach(p => identity.AddClaim(new Claim(ClaimTypes.Role, p)));

            // execute
            var result = _userService.GetRolesFromClaims(identity.Claims);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            result.ForEach(p => Assert.IsTrue(roles.Contains(p)));
            roles.ForEach(p => Assert.IsTrue(result.Contains(p)));
        }

        [TestMethod]
        public void GetRolesFromClaims_NoClaimsNoRoles()
        {
            // setup
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

            // execute
            var result = _userService.GetRolesFromClaims(identity.Claims);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetRolesFromClaims_NullReturnsNoRoles()
        {
            // execute
            var result = _userService.GetRolesFromClaims(null);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetUserNameFromClaims_Success()
        {
            // setup
            var userName = "someUser";
            var firstName = "first";
            var lastName = "last";
            var id = "dfsdvastjy";
            var roles = new List<string> { "ROLE1", "ROLE2" };
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, firstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, lastName));
            roles.ForEach(p => identity.AddClaim(new Claim(ClaimTypes.Role, p)));

            // execute
            var result = _userService.GetUserNameFromClaims(identity.Claims);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result);
        }

        [TestMethod]
        public void GetUserNameFromClaims_NoUserName_ReturnsNull()
        {
            // setup
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

            // execute
            var result = _userService.GetUserNameFromClaims(identity.Claims);

            // assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetUserNameFromClaims_NullReturnsNull()
        {
            // execute
            var result = _userService.GetUserNameFromClaims(null);

            // assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ValidatePassword_PasswordValid_Success()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockPasswordValidator.Setup(p => p.ValidateAsync(It.IsAny<UserManager<User>>(), user, password))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ValidatePassword(user.UserName, password);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ValidatePassword_PasswordInvalid_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockPasswordValidator.Setup(p => p.ValidateAsync(It.IsAny<UserManager<User>>(), user, password))
                .Returns(Task.FromResult(IdentityResult.Failed(new [] { new IdentityError() { Description = "Bad!" } }))).Verifiable();

            // execute
            var result = await _userService.ValidatePassword(user.UserName, password);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ValidatePassword_NoSuchUser_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockPasswordValidator.Setup(p => p.ValidateAsync(It.IsAny<UserManager<User>>(), null, password))
                .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } }))).Verifiable();

            // execute
            var result = await _userService.ValidatePassword(user.UserName, password);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ValidatePassword_ThrowsException_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Throws(new Exception()).Verifiable();
            //_mockPasswordValidator.Setup(p => p.ValidateAsync(It.IsAny<UserManager<User>>(), null, password))
            //    .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } }))).Verifiable();

            // execute
            var result = await _userService.ValidatePassword(user.UserName, password);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ResetUserPassword_Success()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            var token = "fhgfwsfdfsdhg";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GeneratePasswordResetTokenAsync(user)).Returns(Task.FromResult(token)).Verifiable();
            _mockUserManager.Setup(p => p.ResetPasswordAsync(user, token, password)).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ResetUserPassword(user.UserName, password);

            // assert
            Assert.IsTrue(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ResetUserPassword_NoSuchUser_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            
            // execute
            var result = await _userService.ResetUserPassword(user.UserName, password);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.GeneratePasswordResetTokenAsync(It.IsAny<User>()), Times.Never);
            _mockUserManager.Verify(p => p.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ResetUserPassword_ResetReturnsFailure_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            var token = "fhgfwsfdfsdhg";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GeneratePasswordResetTokenAsync(user)).Returns(Task.FromResult(token)).Verifiable();
            _mockUserManager.Setup(p => p.ResetPasswordAsync(user, token, password))
                .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } }))).Verifiable();

            // execute
            var result = await _userService.ResetUserPassword(user.UserName, password);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ResetUserPassword_ThrowsException_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var password = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GeneratePasswordResetTokenAsync(user)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.ResetUserPassword(user.UserName, password);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_NoSuchUser_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };
            var systemRoles = Queryable.AsQueryable(systemRolesAsList);

            _mockRoleManager.Setup(p => p.Roles).Returns(systemRoles).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            
            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            _mockUserManager.Verify(p => p.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_NoSuchRole_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };
            var systemRoles = Queryable.AsQueryable(systemRolesAsList);

            _mockRoleManager.Setup(p => p.Roles).Returns(systemRoles).Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.FindByNameAsync(It.IsAny<string>()), Times.Never);
            _mockUserManager.Verify(p => p.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            _mockUserManager.Verify(p => p.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_AddsNewRoles_NoRemove()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var existingRoles = new List<string> { "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.AddToRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsTrue(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_RemovesMissingRoles_NoAdd()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "OTHERROLE" };
            var existingRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.RemoveFromRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles); 

            // assert
            Assert.IsTrue(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.AddToRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_AddsAndRemoves()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1" };
            var existingRoles = new List<string> { "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.AddToRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            _mockUserManager.Setup(p => p.RemoveFromRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "OTHERROLE") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsTrue(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ChangeUserRoles_RoleAddReturnsFailure_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var existingRoles = new List<string> { "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.AddToRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } })))
                            .Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_RoleAddExceptions_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var existingRoles = new List<string> { "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.AddToRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Throws(new Exception())
                            .Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_RoleRemoveReturnsFailure_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "OTHERROLE" };
            var existingRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.RemoveFromRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } })))
                            .Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.AddToRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserRoles_RoleRemoveExceptions_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var newRoles = new List<string> { "OTHERROLE" };
            var existingRoles = new List<string> { "ROLE1", "OTHERROLE" };
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };

            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(user)).Returns(Task.FromResult((IList<string>)existingRoles)).Verifiable();
            _mockUserManager.Setup(p => p.RemoveFromRolesAsync(user, It.Is<List<string>>(x => x.Count(y => y == "ROLE1") == 1
                                                                                       && x.Count() == 1)))
                            .Throws(new Exception())
                            .Verifiable();

            // execute
            var result = await _userService.ChangeUserRoles(user.UserName, newRoles);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.AddToRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task ListRoles_Success()
        {
            // setup
            var systemRolesAsList = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Name = "ROLE1"
                },
                new IdentityRole()
                {
                    Name = "ROLE2"
                },
                new IdentityRole()
                {
                    Name = "OTHERROLE"
                }
            };
            _mockRoleManager.Setup(p => p.Roles).Returns(Queryable.AsQueryable(systemRolesAsList)).Verifiable();

            // execute
            var result = await _userService.ListRoles();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(systemRolesAsList.Count, result.Count);
            Assert.IsTrue(systemRolesAsList.All(p => result.Contains(p.Name)));
            Assert.IsTrue(result.All(p => systemRolesAsList.Any(x => p == x.Name)));
        }

        [TestMethod]
        public async Task ListRoles_ThrowsException()
        {
            // setup
            _mockRoleManager.Setup(p => p.Roles).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.ListRoles();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task ChangeUserEmail_Success()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var email = "asdf";
            var token = "fhgfwsfdfsdhg";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GenerateChangeEmailTokenAsync(user, email)).Returns(Task.FromResult(token)).Verifiable();
            _mockUserManager.Setup(p => p.ChangeEmailAsync(user, email, token)).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // execute
            var result = await _userService.ChangeUserEmail(user.UserName, email);

            // assert
            Assert.IsTrue(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ChangeUserEmail_NoSuchUser_ReturnsFalse()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var email = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userService.ChangeUserEmail(user.UserName, email);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.GenerateChangeEmailTokenAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserManager.Verify(p => p.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeUserEmail_ChangeReturnsFailure_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var email = "asdf";
            var token = "fhgfwsfdfsdhg";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GenerateChangeEmailTokenAsync(user, email)).Returns(Task.FromResult(token)).Verifiable();
            _mockUserManager.Setup(p => p.ChangeEmailAsync(user, email, token))
                .Returns(Task.FromResult(IdentityResult.Failed(new[] { new IdentityError() { Description = "Bad!" } }))).Verifiable();

            // execute
            var result = await _userService.ChangeUserEmail(user.UserName, email);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
        }

        [TestMethod]
        public async Task ChangeUserEmail_ThrowsException_Failure()
        {
            // setup
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var email = "asdf";
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GenerateChangeEmailTokenAsync(user, email)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userService.ResetUserPassword(user.UserName, email);

            // assert
            Assert.IsFalse(result);
            _mockUserManager.Verify();
            _mockUserManager.Verify(p => p.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
