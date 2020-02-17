using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserControllerTests
    {
        private UserController _userController;
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IOptions<CustomOptions>> _mockOptions;
        private IMapper _mapper;

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());

            _userController = new UserController(_mockLogger.Object, _mapper, _mockUserService.Object, _mockOptions.Object);
        }

        [TestMethod]
        public async Task Authenticate_Success()
        {
            // setup
            var userName = "myuserName";
            var password = "mypassword";
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName,
                Token = "fhgdfsa"
            };
            _mockUserService.Setup(p => p.Authenticate(userName, password)).Returns(Task.FromResult(user)).Verifiable();

            // execute
            var result = await _userController.Authenticate(new AuthenticationRequest() { Username = userName, Password = password }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("Authorized", authResult.Status);
            Assert.AreEqual(user.Token, authResult.Token);
        }

        [TestMethod]
        public async Task Authenticate_Failure()
        {
            // setup
            var userName = "myuserName";
            var password = "mypassword";
            _mockUserService.Setup(p => p.Authenticate(userName, password)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userController.Authenticate(new AuthenticationRequest() { Username = userName, Password = password }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("User not found or password did not match", authResult.Status);
            Assert.IsTrue(String.IsNullOrEmpty(authResult.Token));
        }

        [TestMethod]
        public async Task Authenticate_ThrowsException()
        {
            // setup
            var userName = "myuserName";
            var password = "mypassword";
            _mockUserService.Setup(p => p.Authenticate(userName, password)).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userController.Authenticate(new AuthenticationRequest() { Username = userName, Password = password }) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
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
            _mockUserService.Setup(p => p.GetUserByEmail(user.Email)).Returns(Task.FromResult(user)).Verifiable();

            // execute
            var result = await _userController.GetUserByEmailAddress(user.Email) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var resultObject = result.Value as UserViewModel;
            Assert.AreEqual(user.Email, resultObject.Email);
            Assert.AreEqual(user.FirstName, resultObject.FirstName);
            Assert.AreEqual(user.LastName, resultObject.LastName);
            Assert.AreEqual(user.UserName, resultObject.UserName);
            _mockUserService.Verify();
        }

        [TestMethod]
        public async Task GetUserByEmail_HandlesNull_ReturnsNull()
        {
            _mockUserService.Setup(p => p.GetUserByEmail(It.IsAny<string>())).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userController.GetUserByEmailAddress("something") as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var resultObject = result.Value as UserViewModel;
            Assert.IsNull(resultObject);
            _mockUserService.Verify();
        }

        [TestMethod]
        public async Task GetUserByEmail_ThrowsException()
        {
            _mockUserService.Setup(p => p.GetUserByEmail(It.IsAny<string>())).Throws(new Exception()).Verifiable();

            // execute
            var result = await _userController.GetUserByEmailAddress("something") as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.Verify();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordModelNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = null;

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = null };

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordEmpty_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "" };

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_UserNameNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };

            var result = await _userController.UpdateUserPassword(null, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_UserNameEmpty_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };

            var result = await _userController.UpdateUserPassword("", passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_NoSuchUser_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_NotInRoleForUpdatingAnotherUser_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordDoesntValidate_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(new List<string>() { "Error!" })).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserPassword_Exception_500()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
        }

        [TestMethod]
        public async Task UpdateUserPassword_UpdatesOwnPassword_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserPassword_UpdatesOtherPassword_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PHOAdmin" };
            PasswordChangeViewModel passwordChangeViewModel = new PasswordChangeViewModel() { NewPassword = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(user.UserName, passwordChangeViewModel.NewPassword))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserPassword(user.UserName, passwordChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_RoleModelNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = null;

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserRoles(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UserNameNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };

            var result = await _userController.UpdateUserRoles(null, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserRoles(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UserNameEmpty_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };

            var result = await _userController.UpdateUserRoles("", RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserRoles(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserRoles_NoSuchUser_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserRoles(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserRoles_RoleDoesntExist_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string> { "WHOOPS" } };
            var existingRoles = new List<string> { "PracticeMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserRoles(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserRoles_Exception_500()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PracticeMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.IsAny<List<string>>()))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOwnRole_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PracticeMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.IsAny<List<string>>()))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOwnRole_RoleListIsNull_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = null };
            var existingRoles = new List<string> { "PracticeMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.IsAny<List<string>>()))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_PHOAdmin_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PHOAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PracticeMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 0)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_PHOAdmin_CanAddPhoMember_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PHOAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string> { "PHOMember" } };
            var existingRoles = new List<string>();
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 1)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_PHOAdmin_CanRemovePHOMember_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PHOAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PHOMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 0)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_NotPHOAdmin_CantRemovePHOMember()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PracticeAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PHOMember" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 1)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_NotPHOAdmin_CantAddPHOMember()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PracticeAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() { "PHOMember" } };
            var existingRoles = new List<string>();
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 0)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_NotPHOAdmin_CantRemovePHOAdmin()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PracticeAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() };
            var existingRoles = new List<string> { "PHOAdmin" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 1)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserRoles_UpdatesOtherRole_NotPHOAdmin_CantAddPHOAdmin()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PracticeAdmin" };
            RoleChangeViewModel RoleChangeViewModel = new RoleChangeViewModel() { NewRoles = new List<string>() { "PHOAdmin" } };
            var existingRoles = new List<string>();
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.GetRoles(user.UserName)).Returns(Task.FromResult(existingRoles)).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserRoles(user.UserName, It.Is<List<string>>(x => x.Count == 0)))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserRoles(user.UserName, RoleChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserEmail_EmailModelNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = null;

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_EmailNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = null };

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_EmailEmpty_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "" };

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_UserNameNull_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };

            var result = await _userController.UpdateUserEmail(null, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_UserNameEmpty_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };

            var result = await _userController.UpdateUserEmail("", EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_NoSuchUser_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_NotInRoleForUpdatingAnotherUser_BadRequest()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
            _mockUserService.Verify(p => p.ChangeUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateUserEmail_Exception_500()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserEmail(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value as string));
        }

        [TestMethod]
        public async Task UpdateUserEmail_UpdatesOwnEmail_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(user.UserName).Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(new List<string>()).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserEmail(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task UpdateUserEmail_UpdatesOtherEmail_Success()
        {
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = "myUser"
            };
            var roleList = new List<string> { "PHOAdmin" };
            EmailChangeViewModel EmailChangeViewModel = new EmailChangeViewModel() { NewEmailAddress = "asdf" };
            _mockUserService.Setup(p => p.GetUserByUserName(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("someoneelse").Verifiable();
            _mockUserService.Setup(p => p.GetRolesFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleList).Verifiable();
            _mockUserService.Setup(p => p.ValidatePassword(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Returns(Task.FromResult(new List<string>())).Verifiable();
            _mockUserService.Setup(p => p.ChangeUserEmail(user.UserName, EmailChangeViewModel.NewEmailAddress))
                .Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserEmail(user.UserName, EmailChangeViewModel) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue((bool)result.Value);
        }

        [TestMethod]
        public async Task ListRoles_Success()
        {
            var roles = new List<string> { "FIRST", "SECOND" };
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(roles)).Verifiable();

            var result = await _userController.ListRoles() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var resultList = result.Value as List<string>;
            Assert.IsTrue(resultList.TrueForAll(x => roles.Contains(x)));
            Assert.IsTrue(roles.TrueForAll(x => resultList.Contains(x)));
        }

        [TestMethod]
        public async Task ListRoles_ThrowsException()
        {
            _mockUserService.Setup(p => p.ListRoles()).Throws(new Exception()).Verifiable();

            var result = await _userController.ListRoles() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
